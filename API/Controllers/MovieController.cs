using System;
using System.Threading.Tasks;
using API.DTO;
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
                people.Add(person);
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
            _context.People.AddRange(people);

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

        [HttpPut("{id}/addEmployee")]
        public async Task<IActionResult> AddActorToMovie(Guid id, [FromBody] ICollection<MovieEmployeeDTO> newMovieEmployees)
        {
            // Return if request was sent with no new employees
            if (newMovieEmployees.Count == 0) return ValidationProblem("No new movie employees added");

            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound("Cannot add actor to requested Movie: Movie not found");

            // Sanitize newMovieEmployees
            ICollection<MovieEmployeeDTO> sanitizedNewME = MovieHelpers.SanitizeEmployeesInput(newMovieEmployees);

            // Temporary list to store new database entires
            List<Person> people = new List<Person>();

            foreach (var employee in sanitizedNewME)
            {
                // Check if the person already exists in the database.
                // If not, continue and don't re-add to the database.
                var personInDB = await _context.People.SingleOrDefaultAsync(p => (p.FirstName == employee.FirstName && p.LastName == employee.LastName));
                if (personInDB == null)
                {
                    continue;
                }

                // Create the person and store in an List
                // We will insert all at the same time as opposed to per person
                var person = new Person
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName
                };

                movie.Employees.Add(new MovieEmployee { Person = person });
                people.Add(person);
            }

            // Save new Person entires to the database
            await _context.People.AddRangeAsync(people);
            _context.Update(movie);
            //_context.Movies.FirstOrDefaultAsync

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Error adding to database");

            return Ok("Updated employees list in the movie");

        }
    }
}