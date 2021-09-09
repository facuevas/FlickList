using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MovieEmployee>(x => x.HasKey(me => new { me.PersonId, me.MovieId }));

            modelBuilder.Entity<MovieEmployee>()
                .HasOne(p => p.Person)
                .WithMany(m => m.Movies)
                .HasForeignKey(p => p.PersonId);

            modelBuilder.Entity<MovieEmployee>()
                .HasOne(m => m.Movie)
                .WithMany(p => p.Employees)
                .HasForeignKey(m => m.MovieId);

        }
    }
}