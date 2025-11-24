using LOAN_Web_API.Models.Enums;

namespace LOAN_Web_API.Models.DTOs
{
    // მომხმარებლის მიერ სესხის განახლებისთვის (სტატუსის გარდა)
    public class LoanUpdateDTO
    {
        // ID საჭიროა კონტროლერში, მაგრამ აქ არ გვჭირდება, რადგან URL-დან მოვა
        public LoanType LoanType { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public int PeriodInMonths { get; set; }
    }
}
