using LOAN_Web_API.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace LOAN_Web_API.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public string MonthlyIncome { get; set; } = null!;
        public bool IsBlocked { get; set; } = false;
        public string PasswordHash { get; set; } = null!;
        public Role Role { get; set; } = Role.User;
        public ICollection<Loan>? Loans { get; set; }
    }
}
