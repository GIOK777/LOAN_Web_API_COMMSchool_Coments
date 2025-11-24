using LOAN_Web_API.Models.DTOs;
using LOAN_Web_API.Utilities;

namespace LOAN_Web_API.Interfaces
{
    // პასუხისმგებელია მომხმარებლის მონაცემების მართვაზე
    public interface IUserService
    {
        Task<CustomResult<UserResponseDTO>> GetUserByIdAsync(int id);
        Task<CustomResult<bool>> BlockUserAsync(int userId, bool isBlocked); // ადმინისტრატორის ფუნქცია
    }
}
