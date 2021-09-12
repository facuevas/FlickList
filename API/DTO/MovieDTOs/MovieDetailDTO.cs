using System;
using System.Collections.Generic;
using Domain;

// MovieDetail DTO
// We use this to map our Movie model
// This helps us prevent cycles

namespace API.DTO.MovieDTOs
{
    public class MovieDetailDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int RunTime { get; set; }
        public ICollection<MovieEmployeeDTO> Employees { get; set; }
    }
}