using LOAN_Web_API.Models.Entities;

namespace LOAN_Web_API.Interfaces
{
    public interface IJwtGenerator
    {
        // აგენერირებს JWT ტოკენს მომხმარებლის ობიექტის საფუძველზე
        string GenerateToken(User user);
    }
}
