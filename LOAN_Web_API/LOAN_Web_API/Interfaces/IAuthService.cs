using LOAN_Web_API.Models.DTOs;
using LOAN_Web_API.Models.Entities;
using LOAN_Web_API.Utilities;

namespace LOAN_Web_API.Interfaces
{
    public interface IAuthService
    {
        //Task<User> RegisterAsync(UserRegisterDTO userRegisterDTO);
        //Task<string> LoginAsync(UserLoginDTO userLoginDTO); // დააბრუნებს JWT-ს

        Task<CustomResult<UserResponseDTO>> RegisterAsync(UserRegisterDTO userRegisterDto);
        Task<CustomResult<string>> LoginAsync(UserLoginDTO userLoginDto);
    }
}
