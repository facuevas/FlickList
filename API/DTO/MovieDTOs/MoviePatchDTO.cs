// This DTO is used to patch new actors into the Movie.Employees field

namespace API.DTO.MovieDTOs
{
    public class MoviePatchDTO
    {
        public string Operation { get; set; }
        public string Title { get; set; }
    }
}