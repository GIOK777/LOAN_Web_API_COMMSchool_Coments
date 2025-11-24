using AutoMapper;
using LOAN_Web_API.Models.DTOs;
using LOAN_Web_API.Models.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LOAN_Web_API.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mappings
            CreateMap<UserRegisterDTO, User>(); // რეგისტრაციის დროს DTO -> Entity
            CreateMap<User, UserResponseDTO>(); // ინფორმაციის დაბრუნებისას Entity -> DTO

            // Loan Mappings
            CreateMap<LoanRequestDTO, Loan>();      // სესხის მოთხოვნის დროს
            CreateMap<LoanUpdateDTO, Loan>();       // სესხის განახლების დროს
            CreateMap<Loan, LoanResponseDTO>();     // სესხის დაბრუნების დროს
        }
    }
}
