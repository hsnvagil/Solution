using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace App.API.Middlewares;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull {
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken) {
        if (!validators.Any()) return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var errorsList = validators
            .SelectMany(validator => validator.Validate(context).Errors)
            .Where(error => error != null)
            .Select(error => new ValidationFailure {
                PropertyName = error.PropertyName,
                ErrorMessage = error.ErrorMessage
            })
            .Distinct()
            .ToList();
        
        if (errorsList.Count > 0) throw new ValidationException(errorsList);

        return await next(cancellationToken);
    }
}