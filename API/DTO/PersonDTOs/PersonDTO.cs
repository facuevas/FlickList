using System;

namespace API.DTO.PersonDTOs
{
    public class PersonDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}