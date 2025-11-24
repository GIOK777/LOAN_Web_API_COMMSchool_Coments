using LOAN_Web_API.Models.Enums;

namespace LOAN_Web_API.Models.DTOs
{
    // მომხმარებლის ინფორმაციის დასაბრუნებლად
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public decimal MonthlyIncome { get; set; }
        public bool IsBlocked { get; set; }
        public Role Role { get; set; }
    }
}
