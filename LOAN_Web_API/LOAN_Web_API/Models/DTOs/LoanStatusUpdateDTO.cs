using LOAN_Web_API.Models.Enums;

namespace LOAN_Web_API.Models.DTOs
{
    // ადმინისტრატორის მიერ სტატუსის შესაცვლელად (SRP)
    public class LoanStatusUpdateDTO
    {
        public LoanStatus Status { get; set; }
    }
}
