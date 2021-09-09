using Domain;
using FluentValidation;

namespace API.Validator
{
    public class MovieValidator : AbstractValidator<Movie>
    {
        public MovieValidator()
        {
            RuleFor(m => m.Title).NotEmpty();
            RuleFor(m => m.ReleaseDate).NotEmpty();
            RuleFor(m => m.Runtime).NotEmpty();
        }
    }
}