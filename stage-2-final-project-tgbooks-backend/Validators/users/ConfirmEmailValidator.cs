using FluentValidation;
using stage_2_final_project_tgbooks_backend.Requests.Models.Users;

namespace stage_2_final_project_tgbooks_backend.Validators.users
{
    public class ConfirmEmailValidator : AbstractValidator<ConfirmEmail>
    {
        public ConfirmEmailValidator()
        {
            RuleFor(em => em.Email)
                .NotEmpty().WithMessage("Email can't be empty")
                .EmailAddress().WithMessage("Email format is not valid");

            RuleFor(em => em.Code)
                .NotEmpty().WithMessage("Code can't be empty")
                .Matches(@"^\d{4}$").WithMessage("Code must be exactly 4 digits");
        }
    }
}
