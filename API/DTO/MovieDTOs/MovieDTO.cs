using System;

namespace API.DTO
{
    public class MovieDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Runtime { get; set; }
    }
}