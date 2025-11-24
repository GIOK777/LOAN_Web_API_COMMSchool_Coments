using FluentValidation;
using LOAN_Web_API.Models.DTOs;

namespace LOAN_Web_API.Validators
{
    public class LoginDtoValidator : AbstractValidator<UserLoginDTO>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}
