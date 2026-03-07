using FluentValidation;
using stage_2_final_project_tgbooks_backend.Requests.Models.Users;

namespace stage_2_final_project_tgbooks_backend.Validators.users
{
    public class AddUserValidator : AbstractValidator<AddUser>
    {
        public AddUserValidator()
        {
            RuleFor(u => u.FirstName)
                .NotEmpty().WithMessage("User first name can't be empty.")
                .MinimumLength(2).WithMessage("User first name must be at least 2 characters long")
                .MaximumLength(60).WithMessage("First name length should be less than than 61");

            RuleFor(u => u.LastName)
                .NotEmpty().WithMessage("User last name can't be empty.")
                .MinimumLength(2).WithMessage("User last name must be at least 2 characters long")
                .MaximumLength(60).WithMessage("Last name length should be less than 61");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email can't be empty")
                .EmailAddress().WithMessage("Email format is not valid");

            RuleFor(u => u.DateOfBirth)
                .LessThan(DateOnly.FromDateTime(DateTime.Today))
                    .WithMessage("Person can't be born in the future")
                .GreaterThan(DateOnly.FromDateTime(DateTime.Today.AddYears(-160)))
                    .WithMessage("Person can't be more than 160 years old");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password can't be empty")
                .Matches(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$").WithMessage("Password format incorrect");

        }
    }
}
