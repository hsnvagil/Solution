using FluentValidation;

namespace App.Application.UseCases.Queries.GetAvailableHomes;

public class GetAvailableHomesRequestValidator : AbstractValidator<GetAvailableHomesRequest> {
    public GetAvailableHomesRequestValidator() {
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Başlanğıc tarixi tələb olunur")
            .Must(BeValidDate).WithMessage("Başlanğıc tarixi düzgün formatda olmalıdır (yyyy-MM-dd)")
            .Must(NotInThePast).WithMessage("Başlanğıc tarixi bu gün və ya gələcək gün olmalıdır.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("Bitmə tarixi tələb olunur")
            .Must(BeValidDate).WithMessage("Bitmə tarixi düzgün formatda olmalıdır (yyyy-MM-dd)");

        RuleFor(x => x)
            .Must(HaveValidDateRange)
            .WithMessage("Başlanğıc tarixi bitmə tarixindən kiçik və ya ona bərabər olmalıdır.");
    }

    private bool BeValidDate(string dateStr) {
        return DateOnly.TryParseExact(dateStr, "yyyy-MM-dd", out _);
    }
    
    private bool NotInThePast(string dateStr) {
        if (!DateOnly.TryParseExact(dateStr, "yyyy-MM-dd", out var date)) return false;
        return date >= DateOnly.FromDateTime(DateTime.Today);
    }

    private bool HaveValidDateRange(GetAvailableHomesRequest request) {
        if (!DateOnly.TryParseExact(request.StartDate, "yyyy-MM-dd", out var startDate)) return false;
        if (!DateOnly.TryParseExact(request.EndDate, "yyyy-MM-dd", out var endDate)) return false;
        return startDate <= endDate;
    }
}