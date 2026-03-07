using FluentValidation;
using stage_2_final_project_tgbooks_backend.Requests.Models.Users;

namespace stage_2_final_project_tgbooks_backend.Validators.users
{
    public class EditUserNameValidator : AbstractValidator<EditUserName>
    {
        public EditUserNameValidator()
        {
            RuleFor(u => u.FirstName)
                .NotEmpty().WithMessage("User first name can't be empty.")
                .MinimumLength(2).WithMessage("User first name must be at least 2 characters long")
                .MaximumLength(60).WithMessage("First name length should be greater than 50");

            RuleFor(u => u.LastName)
                    .NotEmpty().WithMessage("User last name can't be empty.")
                    .MinimumLength(2).WithMessage("User last name must be at least 2 characters long")
                    .MaximumLength(60).WithMessage("Last name length should be greater than 50");

        }
    }
}
