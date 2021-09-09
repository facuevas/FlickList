using System;
using System.Collections.Generic;

namespace Domain
{
    public class Movie
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Runtime { get; set; }
        public ICollection<MovieEmployee> Employees { get; set; } = new List<MovieEmployee>();
    }
}