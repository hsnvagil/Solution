using App.Application.Mapper;
using App.Core.Interfaces;
using AutoMapper;
using MediatR;

namespace App.Application.UseCases.Queries.GetAvailableHomes;

public record GetAvailableHomesRequest(string StartDate, string EndDate)
    : IRequest<List<GetAvailableHomesResponse>>;

public class GetAvailableHomesQueryHandler(IMapper mapper, IHomeDataProvider provider)
    : IRequestHandler<GetAvailableHomesRequest, List<GetAvailableHomesResponse>> {
    public async Task<List<GetAvailableHomesResponse>> Handle(GetAvailableHomesRequest request,
        CancellationToken cancellationToken) {
        var range = mapper.Map<DateRange>(request);

        var homes = provider.GetAllHomes();
        var dateRange = Enumerable.Range(0, range.End.DayNumber - range.Start.DayNumber + 1)
            .Select(i => range.Start.AddDays(i))
            .ToList();

        var filtered = homes
            .Where(kvp => dateRange.All(d => kvp.Value.Contains(d)))
            .ToList();

        return mapper.Map<List<GetAvailableHomesResponse>>(filtered);
    }
}