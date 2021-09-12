using System;
using System.Collections.Generic;

namespace API.DTO.PersonDTOs
{
    public class PersonDetailDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<MovieEmployedDTO> Movies { get; set; }
    }
}