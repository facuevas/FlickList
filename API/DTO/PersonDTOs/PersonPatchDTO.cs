namespace API.DTO.PersonDTOs
{
    public class PersonPatchDTO
    {
        public string Operation { get; set; }
        public PersonUpdateDTO Person { get; set; }
    }
}