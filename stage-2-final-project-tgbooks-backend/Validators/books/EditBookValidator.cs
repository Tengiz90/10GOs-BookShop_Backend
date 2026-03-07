using FluentValidation;
using stage_2_final_project_tgbooks_backend.Requests.Models.Books;

namespace stage_2_final_project_tgbooks_backend.Validators.books
{
    public class EditBookValidator : AbstractValidator<EditBook>
    {
        public EditBookValidator()
        {
            RuleForEach(b => b.AuthorNames).ChildRules(author =>
            {
                author.RuleFor(x => x)
                    .NotEmpty().WithMessage("Author name cannot be empty")
                    .MinimumLength(5).WithMessage("Author name must be at least 5 characters long")
                    .MaximumLength(122).WithMessage("Author name must be less than 123 characters long");
            });

            RuleFor(b => b.Title)
                .NotEmpty().WithMessage("Book title can't be empty");
        }
    }
}
