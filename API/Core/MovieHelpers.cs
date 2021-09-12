using System.Collections.Generic;
using API.DTO.MovieDTOs;

namespace API.Core
{
    public static class MovieHelpers
    {
        public static ICollection<MovieEmployeeDTO> SanitizeEmployeesInput(ICollection<MovieEmployeeDTO> me)
        {
            HashSet<MovieEmployeeDTO> uniqueEmployees = new HashSet<MovieEmployeeDTO>(new MovieEmployeeDTOComparer());

            // Remove duplicates using a HashSet
            foreach (var employee in me)
            {
                uniqueEmployees.Add(employee);
            }

            return uniqueEmployees;
        }
    }
}