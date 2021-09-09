using System;

namespace Domain
{
    public class MovieEmployee
    {
        public Guid PersonId { get; set; }
        public Person Person { get; set; }
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}