using LOAN_Web_API.Models.DTOs;
using LOAN_Web_API.Models.Entities;
using LOAN_Web_API.Models.Enums;
using LOAN_Web_API.Utilities;

namespace LOAN_Web_API.Interfaces
{
    // პასუხისმგებელია სესხის ლოგიკაზე
    public interface ILoanService
    {
        //Task<Loan> CreateLoanAsync(int userId, CreateLoanDTO createLoanDTO, Role role);


        //Task<IEnumerable<Loan>> GetLoansAsync(int userId, Role role);
        ////Task<List<Loan>> GetUserLoansAsync(int userId);
        ////Task<Loan> GetLoanByIdAsync(int userId, int loanId, string role);


        ////Task<Loan> UpdateLoanAsync(int userId, int loanId, UpdateLoanDto updateLoanDTO);
        //Task<Loan> UpdateLoanAsync(int userId, int loanId, LoanRequestDTO updateLoanDTO, Role role);


        ////Task<bool> DeleteLoanAsync(int userId, int loanId);
        //Task<bool> DeleteLoanAsync(int userId, int loanId, Role role);

        // User Methods
        Task<CustomResult<LoanResponseDTO>> AddLoanAsync(int userId, LoanRequestDTO loanRequestDTO);
        Task<CustomResult<IEnumerable<LoanResponseDTO>>> GetUserLoansAsync(int userId);
        Task<CustomResult<LoanResponseDTO>> UpdateLoanAsync(int userId, int loanId, LoanUpdateDTO loanUpdateDTO);
        Task<CustomResult<bool>> DeleteLoanAsync(int userId, int loanId);

        // Admin Methods
        Task<CustomResult<LoanResponseDTO>> UpdateLoanStatusAsync(int loanId, LoanStatusUpdateDTO loanStatusUpdateDTO);
        Task<CustomResult<IEnumerable<LoanResponseDTO>>> GetAllLoansAsync();
        Task<CustomResult<bool>> DeleteLoanAdminAsync(int loanId);
    }
}
