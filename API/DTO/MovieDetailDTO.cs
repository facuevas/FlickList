using System;
using System.Collections.Generic;
using Domain;

namespace API.DTO
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