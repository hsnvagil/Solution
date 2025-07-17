using App.Application.UseCases.Queries.GetAvailableHomes;
using MediatR;

namespace App.API.Endpoints;

public class HomeEndpoints {
    public static void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("/api/available-homes", GetAvailableHomes);
    }

    private static async Task<IResult> GetAvailableHomes(string startDate, string endDate, IMediator mediator) {
        var query = new GetAvailableHomesRequest(startDate, endDate);
        var result = await mediator.Send(query);
        return Results.Ok(result);
    }
}