using LOAN_Web_API.Models.Enums;

namespace LOAN_Web_API.Models.DTOs
{
    // სესხის ინფორმაციის დასაბრუნებლად
    public class LoanResponseDTO
    {
        public int Id { get; set; }
        public LoanType LoanType { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public int PeriodInMonths { get; set; }
        public LoanStatus Status { get; set; }
        public int UserId { get; set; }
    }
}
