using LOAN_Web_API.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOAN_Web_API.Models.Entities
{
    public class Loan
    {
        public int Id { get; set; }
        public LoanType loanType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public int PeriodInMonths{ get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Processing;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
