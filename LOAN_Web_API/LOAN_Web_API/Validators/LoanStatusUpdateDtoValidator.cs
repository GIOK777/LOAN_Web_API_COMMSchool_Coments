using FluentValidation;
using LOAN_Web_API.Models.DTOs;

namespace LOAN_Web_API.Validators
{
    public class LoanStatusUpdateDtoValidator : AbstractValidator<LoanStatusUpdateDTO>
    {
        public LoanStatusUpdateDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid loan status value.");
        }
    }
}
