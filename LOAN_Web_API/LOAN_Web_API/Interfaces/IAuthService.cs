using LOAN_Web_API.Models.DTOs;

namespace LOAN_Web_API.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDTO registerDTO);
        Task<string> LoginAsync(LoginDTO loginDTO);
    }
}
