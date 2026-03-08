using FluentValidation;
using stage_2_final_project_tgbooks_backend.Requests.Models.Users;

namespace stage_2_final_project_tgbooks_backend.Validators.users
{
    public class SignInUserVallidator : AbstractValidator<SignInUser>
    {
        public SignInUserVallidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .Matches(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$").WithMessage("Password format incorrect");

        }
    }
}

