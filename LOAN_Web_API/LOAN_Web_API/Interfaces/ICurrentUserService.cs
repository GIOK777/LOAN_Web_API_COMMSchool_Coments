using LOAN_Web_API.Models.Enums;

namespace LOAN_Web_API.Interfaces
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        Role Role { get; }
        bool IsAdmin { get; }
        bool IsAuthenticated { get; }
    }
}
