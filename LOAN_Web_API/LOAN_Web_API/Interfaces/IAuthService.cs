using LOAN_Web_API.Models.DTOs;

namespace LOAN_Web_API.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }
}
