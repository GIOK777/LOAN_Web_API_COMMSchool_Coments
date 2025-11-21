using LOAN_Web_API.Interfaces;
using LOAN_Web_API.Models;
using LOAN_Web_API.Models.DTOs;
using LOAN_Web_API.Models.Entities;
using LOAN_Web_API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace LOAN_Web_API.Services
{
    public class LoanService : ILoanService
    {
        private readonly ApplicationDbContext _context;

        public LoanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Loan> CreateLoanAsync(int userId, CreateLoanDTO createLoanDTO)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            if (user.IsBlocked)
                throw new Exception("Blocked users cannot request loans");

            var loan = new Loan
            {
                UserId = userId,
                Amount = createLoanDTO.Amount,
                PeriodInMonths = createLoanDTO.PeriodInMonths,
                Status = LoanStatus.Processing
            };

            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task<IEnumerable<Loan>> GetUserLoansAsync(int userId)
        {
            return await _context.Loans
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<Loan> UpdateLoanAsync(int userId, int loanId, UpdateLoanDto updateLoanDto)
        {
            var loan = await _context.Loans.FindAsync(loanId);

            if (loan == null)
                throw new Exception("Loan not found");

            if (loan.UserId != userId)
                throw new Exception("Access denied");

            if (loan.Status != LoanStatus.Processing)
                throw new Exception("Only loans in Processing state can be updated");

            loan.Amount = updateLoanDto.Amount;
            loan.PeriodInMonths = updateLoanDto.PeriodInMonths;

            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task<bool> DeleteLoanAsync(int userId, int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);

            if (loan == null)
                throw new Exception("Loan not found");

            if (loan.UserId != userId)
                throw new Exception("Access denied");

            if (loan.Status != LoanStatus.Processing)
                throw new Exception("Only loans in Processing state can be deleted");

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
