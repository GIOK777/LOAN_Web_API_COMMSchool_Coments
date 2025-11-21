using LOAN_Web_API.Models.DTOs;
using LOAN_Web_API.Models.Entities;

namespace LOAN_Web_API.Interfaces
{
    public interface ILoanService
    {
        Task<Loan> CreateLoanAsync(int userId, CreateLoanDTO createLoanDTO);
        Task<IEnumerable<Loan>> GetUserLoansAsync(int userId);
        Task<Loan> UpdateLoanAsync(int userId, int loanId, UpdateLoanDto updateLoanDTO);
        Task<bool> DeleteLoanAsync(int userId, int loanId);
    }
}
