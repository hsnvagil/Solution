using App.Core.Common.Constants;
using FluentValidation;

namespace App.Application.UseCases.Queries.GetAvailableHomes;

public class GetAvailableHomesRequestValidator : AbstractValidator<GetAvailableHomesRequest> {
    public GetAvailableHomesRequestValidator() {
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage(ErrorTokens.HomeAvailabilityStartDateRequired)
            .Must(BeValidDate).WithMessage(ErrorTokens.HomeAvailabilityStartDateInvalidFormat)
            .Must(NotInThePast).WithMessage(ErrorTokens.HomeAvailabilityStartDateNotInPast);

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage(ErrorTokens.HomeAvailabilityEndDateRequired)
            .Must(BeValidDate).WithMessage(ErrorTokens.HomeAvailabilityEndDateInvalidFormat);

        RuleFor(x => x)
            .Must(HaveValidDateRange)
            .WithMessage(ErrorTokens.HomeAvailabilityDateRangeInvalid);
    }

    private const string DateFormat = "yyyy-MM-dd";

    private bool BeValidDate(string dateStr) {
        return DateOnly.TryParseExact(dateStr, DateFormat, out _);
    }

    private bool NotInThePast(string dateStr) {
        if (!DateOnly.TryParseExact(dateStr, DateFormat, out var date)) return false;
        return date >= DateOnly.FromDateTime(DateTime.Today);
    }

    private bool HaveValidDateRange(GetAvailableHomesRequest request) {
        if (!DateOnly.TryParseExact(request.StartDate, DateFormat, out var startDate)) return false;
        if (!DateOnly.TryParseExact(request.EndDate, DateFormat, out var endDate)) return false;
        return startDate <= endDate;
    }
}