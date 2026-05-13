using AutoMapper;
using MeetInSport.Application.DTOs.Reservation;
using MeetInSport.Domain.Entities;

namespace MeetInSport.Application.Mappings;

public class ReservationProfile : Profile
{
    public ReservationProfile()
    {
        CreateMap<Reservation, ReservationResponseDto>()
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
        .ForMember(dest => dest.LocationType, opt => opt.MapFrom(src => src.LocationType.ToString()))
        .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.Package != null ? src.Package.PackageName : string.Empty))
        .ForMember(dest => dest.CoachName, opt => opt.MapFrom(src => src.Coach != null ? src.Coach.User.Name : string.Empty))
        .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.Name : string.Empty));

    }
}