using App.Application.UseCases.Queries.GetAvailableHomes;
using App.Core.Entities.Core;
using AutoMapper;

namespace App.Application.Mapper;

public class MappingProfile : Profile {
    public MappingProfile() {
        CreateMap<GetAvailableHomesRequest, DateRange>()
            .ForMember(dest => dest.Start, opt =>
                opt.MapFrom(src => DateOnly.Parse(src.StartDate)))
            .ForMember(dest => dest.End, opt =>
                opt.MapFrom(src => DateOnly.Parse(src.EndDate)));

        CreateMap<KeyValuePair<Home, List<DateOnly>>, GetAvailableHomesResponse>()
            .ForMember(dest => dest.HomeId, opt =>
                opt.MapFrom(src => src.Key.Id))
            .ForMember(dest => dest.HomeName, opt =>
                opt.MapFrom(src => src.Key.Name))
            .ForMember(dest => dest.AvailableSlots, opt =>
                opt.MapFrom(src => src.Value));
    }
}

public class DateRange {
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
}