using System;
using System.Threading.Tasks;
using API.DTO.MovieDTOs;
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
using API.DTO.PersonDTOs;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : FlickListBaseController
    {
        public MovieController(DataContext context, IMapper mapper) : base(context, mapper)
        {
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetMovies()
        {
            // Query DB to list all movies.
            var movies = await _context.Movies.ProjectTo<MovieDTO>(_mapper.ConfigurationProvider).ToListAsync();

            if (movies == null) return NotFound("No movies found");

            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie(Guid id)
        {
            // Query DB to find movie with given id
            var movie = await _context.Movies.ProjectTo<MovieDetailDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound("Movie not found");

            return Ok(movie);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddMovie([FromBody] MovieDetailDTO movieDetailDTO)
        {
            // Check if movie exists in our database
            if (await _context.Movies.AnyAsync(m => m.Title == movieDetailDTO.Title)) return ValidationProblem("Movie is already listed");

            // Add Employees
            var sanitizedEmployees = MovieHelpers.SanitizeEmployeesInput(movieDetailDTO.Employees);

            List<MovieEmployee> employees = new List<MovieEmployee>();
            List<Person> people = new List<Person>();

            // Add all employees to DB if they don't exist already
            foreach (var employee in sanitizedEmployees)
            {
                // Check if the person already exists in the database.
                // If not, continue and don't re-add to the database.
                var personInDB = await _context.People.SingleOrDefaultAsync(p => (p.FirstName == employee.FirstName && p.LastName == employee.LastName));
                if (personInDB != null)
                {
                    employees.Add(new MovieEmployee { Person = personInDB });
                    continue;
                }

                // Create the person and store in an List
                // We will insert all at the same time as opposed to per person
                var person = new Person
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName
                };

                employees.Add(new MovieEmployee { Person = person });
                await new PersonController(_context, _mapper).AddNewPerson(person);
            }

            // Create the Movie
            var movie = new Movie
            {
                Title = movieDetailDTO.Title,
                ReleaseDate = movieDetailDTO.ReleaseDate,
                Runtime = movieDetailDTO.RunTime,
                Employees = employees
            };

            // Validate the movie
            ValidationResult validationResult = new MovieValidator().Validate(movie);
            if (!validationResult.IsValid)
            {
                return ValidationProblem(validationResult.ToString());
            }

            // Add movie, employee(s) into the Database (in-memory)
            _context.Movies.Add(movie);

            // Save changes
            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Error adding movie");

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(Guid id)
        {
            var movie = await _context.Movies.FindAsync(id);

            _context.Remove(movie);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) BadRequest("Failed to delete movie");

            return Ok("Movie deleted successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditMovie(Guid id, MovieDetailDTO mv)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null) return NotFound("Movie to edit is not found");

            var updatedMovie = new Movie
            {
                Id = id,
                Title = mv.Title,
                ReleaseDate = mv.ReleaseDate,
                Runtime = mv.RunTime
            };

            // Validate the movie
            ValidationResult validationResult = new MovieValidator().Validate(updatedMovie);
            if (!validationResult.IsValid)
            {
                return ValidationProblem(validationResult.ToString());
            }

            _mapper.Map(updatedMovie, movie);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Error updating Movie");

            return Ok("Movie updated");
        }

        [HttpPatch("{id}/addPerson")]
        public async Task<IActionResult> AddPersonToMovie(Guid id, [FromBody] PersonPatchDTO pu)
        {
            // Check if the movie they are trying to add the person to exists
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound("Movie was not found");

            // Check if Person to add to movie exists in the database.
            var person = await _context.People.FirstOrDefaultAsync(p => (p.FirstName == pu.Person.FirstName && p.LastName == pu.Person.LastName));

            // If person not found, let's create the person
            // Community members can edit them later
            if (person == null)
            {
                person = new Person
                {
                    FirstName = pu.Person.FirstName,
                    LastName = pu.Person.LastName
                };

                _context.People.Add(person);
            }

            // If we are adding a person to the Movie, create a new MovieEmployee entry
            if (pu.Operation == "add")
            {
                // Check to see if the Person already exists in the Movie entity
                // We can do this by seeing if it exists in our join table.
                if (await _context.MovieEmployees.FirstOrDefaultAsync(me => me.MovieId == movie.Id && me.PersonId == person.Id) != null)
                    return ValidationProblem("Movie already exists for this Person");

                MovieEmployee mv = new MovieEmployee
                {
                    Movie = movie,
                    Person = person
                };

                _context.MovieEmployees.Add(mv);

                person.Movies.Add(mv);
                movie.Employees.Add(mv);
            }

            // If we are remving a person from the movie
            if (pu.Operation == "remove")
            {
                MovieEmployee mv = await _context.MovieEmployees.FirstOrDefaultAsync(me => me.MovieId == movie.Id && me.PersonId == person.Id);

                if (mv == null) return NotFound("Person to remove not found");

                _context.MovieEmployees.Remove(mv);

                person.Movies.Remove(mv);
                movie.Employees.Remove(mv);
            }

            // Save result
            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Issue modifying Employee resource from Movie entitiy");

            return Ok("Successfully modified Employee resource from the Movie entity");
        }

        // [HttpPatch("{id}/addPerson")]
        // public async Task<IActionResult> AddPersonToMovie(Guid Id, [FromBody] JsonPatchDocument<Person> patchDocument)
        // {
        //     if (patchDocument == null) return BadRequest("No person to add");

        //     patchDocument.Add
        // }
    }
}