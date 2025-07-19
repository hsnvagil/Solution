using App.Core.Common.Constants;
using App.Core.Common.LogModels;
using App.Core.Common.Responses;
using App.Core.Exceptions;
using App.Core.Interfaces;
using App.Core.ValueObjects;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace App.API.Middlewares;

public class RequestResponseLoggerMiddleware(
    RequestDelegate next,
    IRequestResponseLogger logger,
    IClock clock) {
    private readonly JsonSerializerSettings _serializerSettings = new() {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public async Task InvokeAsync(HttpContext httpContext,
        IRequestResponseLogCreator logCreator) {
        var log = logCreator.Log;

        log.RequestDate = clock.Now;
        var request = httpContext.Request;

        log.RequestMethod = request.Method;
        log.RequestPath = request.Path;
        log.RequestQuery = request.QueryString.ToString();
        log.RequestQueries = FormatQueries(request.QueryString.ToString());
        log.RequestHeaders = FormatHeaders(request.Headers);
        log.RequestBody = await ReadBodyFromRequest(request);
        log.RequestScheme = request.Scheme;
        log.RequestHost = request.Host.ToString();
        log.RequestContentType = request.ContentType;

        var response = httpContext.Response;
        var originalResponseBody = response.Body;

        string jsonResponse;

        try {
            jsonResponse = await GetCurrentJsonResponse(httpContext);

            var result = JsonConvert.DeserializeObject(jsonResponse) ?? new object();

            var apiResponse = new ApiResponse<object>(result, ResultStatus.Success);

            httpContext.Response.ContentType = ApiResponseType.JsonResponse;

            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(apiResponse, _serializerSettings));
        }
        catch (ValidationException exception) {
            LogError(log, exception);
            httpContext.Response.Body = originalResponseBody;
            jsonResponse = await HandleValidationExceptionAsync(httpContext, exception);
        }
        catch (Exception exception) {
            LogError(log, exception);
            httpContext.Response.Body = originalResponseBody;
            jsonResponse = await HandleExceptionAsync(httpContext, exception);
        }

        log.ResponseContentType = response.ContentType;
        log.ResponseStatus = response.StatusCode.ToString();
        log.ResponseHeaders = FormatHeaders(response.Headers);
        log.ResponseBody = jsonResponse;
        log.ResponseDate = clock.Now;

        var contextFeature =
            httpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (contextFeature != null) {
            var exception = contextFeature.Error;
            LogError(log, exception);
        }

        logger.Log(logCreator);
    }

    private async Task<string> GetCurrentJsonResponse(HttpContext context) {
        var currentBody = context.Response.Body;

        try {
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await next(context);

            context.Response.Body = currentBody;

            memoryStream.Seek(0, SeekOrigin.Begin);

            using var streamReader = new StreamReader(memoryStream);
            return await streamReader.ReadToEndAsync();
        }
        finally {
            context.Response.Body = currentBody;
        }
    }

    private async Task<string> HandleExceptionAsync(HttpContext httpContext, Exception exception) {
        var statusCode = GetHttpStatusCode(exception);
        var response = new ApiResponse<object>(exception.Message);

        var jsonResponse = JsonConvert.SerializeObject(response, _serializerSettings);

        await WriteErrorResponseAsync(httpContext, statusCode, jsonResponse);

        return jsonResponse;
    }

    private async Task<string> HandleValidationExceptionAsync(HttpContext httpContext, ValidationException exception) {
        var statusCode = GetHttpStatusCode(exception);
        var response = new ApiResponse<object>(exception.Errors.First().ErrorMessage);

        var jsonResponse = JsonConvert.SerializeObject(response, _serializerSettings);

        await WriteErrorResponseAsync(httpContext, statusCode, jsonResponse);

        return jsonResponse;
    }

    private async Task WriteErrorResponseAsync(HttpContext httpContext, int statusCode, string jsonResponse) {
        httpContext.Response.ContentType = ApiResponseType.JsonResponse;
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsync(jsonResponse);
    }

    private int GetHttpStatusCode(Exception exception) {
        return exception switch {
            ValidationException => StatusCodes.Status422UnprocessableEntity,
            NotFoundException => StatusCodes.Status404NotFound,
            BadRequestException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private void LogError(RequestResponseLog log, Exception exception) {
        log.ExceptionMessage = exception.Message;
        log.ExceptionStackTrace = exception.StackTrace;
    }

    private Dictionary<string, string> FormatHeaders(IHeaderDictionary headers) {
        return headers.ToDictionary(
            h => h.Key,
            h => string.Join(",", h.Value.ToArray())
        );
    }

    private List<KeyValuePair<string, string>> FormatQueries(string queryString) {
        return (from query in queryString.TrimStart('?').Split("&")
            select query.Split("=",2)
            into items
            let key = items.Length != 0 ? items[0] : string.Empty
            let value = items.Length >= 2 ? items[1] : string.Empty
            where !string.IsNullOrEmpty(key)
            select new KeyValuePair<string, string>(key, value)).ToList();
    }

    private async Task<string> ReadBodyFromRequest(HttpRequest request) {
        request.EnableBuffering();
        using var streamReader = new StreamReader(request.Body, leaveOpen: true);
        var requestBody = await streamReader.ReadToEndAsync();
        request.Body.Position = 0;
        return requestBody;
    }
}