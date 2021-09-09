using API.DTO;
using AutoMapper;
using Domain;

namespace API.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Use AutoMapper to map Movie with our MovieDetailDTO
            CreateMap<Movie, MovieDetailDTO>();

            CreateMap<MovieEmployee, MovieEmployeeDTO>()
                .ForMember(me => me.FirstName, o => o.MapFrom(p => p.Person.FirstName))
                .ForMember(me => me.LastName, o => o.MapFrom(p => p.Person.LastName));
        }
    }
}