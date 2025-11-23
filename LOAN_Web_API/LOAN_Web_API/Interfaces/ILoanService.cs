using LOAN_Web_API.Models.DTOs;
using LOAN_Web_API.Models.Entities;

namespace LOAN_Web_API.Interfaces
{
    public interface ILoanService
    {
        Task<Loan> CreateLoanAsync(int userId, CreateLoanDTO createLoanDTO);


        //Task<IEnumerable<Loan>> GetUserLoansAsync(int userId);
        Task<List<Loan>> GetUserLoansAsync(int userId);
        Task<Loan> GetLoanByIdAsync(int userId, int loanId, string role);


        //Task<Loan> UpdateLoanAsync(int userId, int loanId, UpdateLoanDto updateLoanDTO);
        Task<Loan> UpdateLoanAsync(int userId, int loanId, UpdateLoanDTO updateLoanDTO, string role);


        //Task<bool> DeleteLoanAsync(int userId, int loanId);
        Task<bool> DeleteLoanAsync(int userId, int loanId, string role);
    }
}
