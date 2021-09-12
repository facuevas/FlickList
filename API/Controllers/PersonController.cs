using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Domain;
using System.Collections.Generic;
using API.Core;
using FluentValidation.Results;
using API.Validator;
using System.Linq;
using API.DTO.PersonDTOs;
using API.DTO.MovieDTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace API.Controllers
{
    public class PersonController : FlickListBaseController
    {
        public PersonController(DataContext context, IMapper mapper) : base(context, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetPeople()
        {
            var actors = await _context.People.ProjectTo<PersonDTO>(_mapper.ConfigurationProvider).ToListAsync();

            if (actors == null) return NotFound("No actors found");

            return Ok(actors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetailedPerson(Guid id)
        {
            var actor = await _context.People.ProjectTo<PersonDetailDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync(p => p.Id == id);

            if (actor == null) return NotFound("Actor not found");

            return Ok(actor);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewPerson([FromBody] Person person)
        {
            // check if the Person is in our database
            if (await _context.People.AnyAsync(p => (p.FirstName == person.FirstName && p.LastName == person.LastName)))
            {
                return ValidationProblem("Actor already exists in our database");
            }

            // Validate the new Person entry
            ValidationResult validationResult = new PersonValidator().Validate(person);
            if (!validationResult.IsValid)
            {
                return ValidationProblem(validationResult.ToString());
            }

            // Add person to database
            _context.People.Add(person);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Error adding person");

            return Ok("Person added successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(Guid id)
        {
            var person = await _context.People.FindAsync(id);

            if (person == null) return ValidationProblem("Person does not exist in our database");

            _context.Remove(person);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Failed to delete person");

            return Ok("Successfully deleted person");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(Guid id, [FromBody] PersonUpdateDTO pu)
        {
            var personToUpdate = await _context.People.FirstOrDefaultAsync(p => p.Id == id);

            if (personToUpdate == null) return ValidationProblem("Person to update not found");

            _mapper.Map(pu, personToUpdate);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Failed to update person");

            return Ok("Person updated successfully");
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ModifyMoviesInPerson(Guid id, [FromBody] MoviePatchDTO mp)
        {

            // Find the person in our database
            // Check if they exist
            var person = await _context.People.FirstOrDefaultAsync(p => p.Id == id);

            if (person == null) return ValidationProblem("Person not found");

            // Find the movie in our database
            // Check if it exists
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Title == mp.Title);

            // If movie is not found, let's add it
            // Community can edit the proper values after it has been added.
            if (movie == null)
            {
                movie = new Movie
                {
                    Title = mp.Title,
                    Runtime = 0,
                    ReleaseDate = new DateTime(1900, 1, 1)
                };

                _context.Movies.Add(movie);
            }

            if (mp.Operation == "add")
            {
                // Create a new MovieEmployee entry and save it to the database.
                MovieEmployee mv = new MovieEmployee
                {
                    Movie = movie,
                    Person = person
                };

                _context.MovieEmployees.Add(mv);

                // Add the MovieEmployee entry to the movie and the person
                movie.Employees.Add(mv);
                person.Movies.Add(mv);
            }

            if (mp.Operation == "remove")
            {
                MovieEmployee mv = await _context.MovieEmployees.FirstOrDefaultAsync(mv => (mv.MovieId == movie.Id && mv.PersonId == person.Id));

                movie.Employees.Remove(mv);
                person.Movies.Remove(mv);
                _context.MovieEmployees.Remove(mv);
            }

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Error modifying Person movies resource");

            return Ok("Succesfully modified Person movies resource");
        }
    }
}