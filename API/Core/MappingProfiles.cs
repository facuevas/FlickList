using API.DTO.PersonDTOs;
using API.DTO.MovieDTOs;
using AutoMapper;
using Domain;

namespace API.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // For our edits
            CreateMap<Movie, Movie>();
            CreateMap<Person, Person>();

            // Use AutoMapper to map Movie
            // MovieDetailDTO for single, listed movies
            // MovieDTO for overall movies.
            CreateMap<Movie, MovieDTO>();
            CreateMap<Movie, MovieDetailDTO>();


            // Use AutoMapper to map MovieEmployee with our MovieEmployeeDTO
            // Map specifically the first name and the last name
            // Those are the only fields we want
            CreateMap<MovieEmployee, MovieEmployeeDTO>()
                .ForMember(me => me.FirstName, o => o.MapFrom(p => p.Person.FirstName))
                .ForMember(me => me.LastName, o => o.MapFrom(p => p.Person.LastName));

            // Person Mappings
            CreateMap<Person, PersonDTO>();

            CreateMap<Person, PersonDetailDTO>();

            CreateMap<PersonUpdateDTO, Person>();

            CreateMap<MovieEmployee, MovieEmployedDTO>()
                .ForMember(me => me.Title, o => o.MapFrom(m => m.Movie.Title))
                .ForMember(me => me.ReleaseDate, o => o.MapFrom(m => m.Movie.ReleaseDate));
        }
    }
}