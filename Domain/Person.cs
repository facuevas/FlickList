using System;
using System.Collections.Generic;

namespace Domain
{
    public class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<MovieEmployee> Movies { get; set; } = new List<MovieEmployee>();
    }
}