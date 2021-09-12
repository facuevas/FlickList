using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

//MovieEmployee DTO
// We use this to map the MovieEmployee join table.
// This helps us prevent cycles

namespace API.DTO.MovieDTOs
{
    public class MovieEmployeeDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class MovieEmployeeDTOComparer : IEqualityComparer<MovieEmployeeDTO>
    {
        public bool Equals(MovieEmployeeDTO x, MovieEmployeeDTO y)
        {
            return (x.FirstName == y.FirstName) && (x.LastName == y.LastName);
        }

        public int GetHashCode([DisallowNull] MovieEmployeeDTO obj)
        {
            return (obj.FirstName + obj.LastName).GetHashCode();
        }
    }
}