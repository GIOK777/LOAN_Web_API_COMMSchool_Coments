using LOAN_Web_API.Models.Enums;

namespace LOAN_Web_API.Models.Entities
{
    public class Loan
    {
        public int Id { get; set; }
        public LoanType loanType { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public int Period { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Processing;
        public int UserId { get; set; }
        public User user { get; set; } = null!;
    }
}
