using FluentValidation;
using LOAN_Web_API.Models.DTOs;

namespace LOAN_Web_API.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Age).GreaterThanOrEqualTo(18);
            RuleFor(x => x.MonthlyIncome).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }
    }
}
