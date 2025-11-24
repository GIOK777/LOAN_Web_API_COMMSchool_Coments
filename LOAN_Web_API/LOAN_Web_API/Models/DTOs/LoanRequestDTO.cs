using LOAN_Web_API.Models.Enums;

namespace LOAN_Web_API.Models.DTOs
{
    // სესხის მოთხოვნისთვის
    public class LoanRequestDTO
    {
        public LoanType LoanType { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public int PeriodInMonths { get; set; }
    }
}
