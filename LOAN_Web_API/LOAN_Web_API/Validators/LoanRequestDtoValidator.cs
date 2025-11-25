using FluentValidation;
using LOAN_Web_API.Models.DTOs;

namespace LOAN_Web_API.Validators
{
    public class LoanRequestDtoValidator : AbstractValidator<LoanRequestDTO>
    {
        public LoanRequestDtoValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(100).WithMessage("Loan amount must be greater than 100.")
                .LessThanOrEqualTo(100000).WithMessage("Loan amount cannot exceed 100,000.");

            RuleFor(x => x.PeriodInMonths)
                .InclusiveBetween(3, 60).WithMessage("Loan period must be between 3 and 60 months.");

            // Enum-ების შემოწმება, რომ მიღებული მნიშვნელობა იყოს ვალიდური Enum-ში
            RuleFor(x => x.Currency)
                .IsInEnum().WithMessage("Invalid currency type.");

            RuleFor(x => x.LoanType)
                .IsInEnum().WithMessage("Invalid loan type.");
        }
    }
}
