using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context)
        {

            // If we have items in our database, do not generate the seed.
            if (context.Movies.Any() || context.People.Any()) { return; }

            // Create dummy actors
            var people = new List<Person>
            {
                new Person
                {
                    FirstName = "Tobey",
                    LastName = "Maguire",
                },
                new Person
                {
                    FirstName = "Kirsten",
                    LastName = "Dunst"
                },
                new Person
                {
                    FirstName = "James",
                    LastName = "Franco"
                },
                new Person
                {
                    FirstName = "Dave",
                    LastName = "Franco"
                }
            };


            // Create dummy movies
            var movies = new List<Movie>
            {
                new Movie
                {
                    Title = "Spider-Man",
                    ReleaseDate = new DateTime(2002, 5, 3),
                    Runtime = 121,
                    Employees = new List<MovieEmployee>
                    {
                        new MovieEmployee
                        {
                            Person = people[0]
                        },
                        new MovieEmployee
                        {
                            Person = people[1]
                        },
                        new MovieEmployee
                        {
                            Person = people[2]
                        },
                    }
                },
                new Movie
                {
                    Title = "The Disaster Artist",
                    ReleaseDate = new DateTime(2017, 12, 8),
                    Runtime = 104,
                    Employees = new List<MovieEmployee>
                    {
                        new MovieEmployee
                        {
                            Person = people[2]
                        },
                        new MovieEmployee
                        {
                            Person = people[3]
                        }
                    }
                }
            };

            // Save data to the database
            await context.People.AddRangeAsync(people);
            await context.Movies.AddRangeAsync(movies);
            await context.SaveChangesAsync();
        }
    }
}