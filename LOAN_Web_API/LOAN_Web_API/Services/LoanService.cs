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

        public async Task<Loan> CreateLoanAsync(int userId, CreateLoanDTO createLoanDTO, Role role)
        {
            // Admin bypass  
            if (role != Role.Administrator)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user.IsBlocked)
                    throw new Exception("You are blocked and cannot create a loan.");
            }

            var loan = new Loan
            {
                UserId = userId,
                Amount = createLoanDTO.Amount,
                Currency = createLoanDTO.Currency,
                PeriodInMonths = createLoanDTO.PeriodInMonths,
                loanType = createLoanDTO.LoanType,
                Status = LoanStatus.Processing,
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return loan;
        }



        // როცა უბრალოდ "ვბრუნებ კოლექციას" და nothing more -  ვიყენებთ IEnumerable
        public async Task<IEnumerable<Loan>> GetLoansAsync(int userId, Role role)
        {
            if (role == Role.Administrator)
                return await _context.Loans.ToListAsync();

            return await _context.Loans
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }


        public async Task<Loan> UpdateLoanAsync(int currentUserId, int loanId, LoanRequestDTO updateLoanDTO, Role role)
        {
            var loan = await _context.Loans.FindAsync(loanId);

            if (loan == null)
                throw new Exception("Loan not found.");


            // თუ არაა ადმინი და ცდილობს სხვის სესხს
            if (role == Role.User && loan.UserId != currentUserId)
                throw new Exception("Forbidden");


            //// Admin → შეუძლია განახლება ყველას loan-ზე
            //if (role != Role.Administrator)
            //{
            //    // ოღონდ უბრალო User → მხოლოდ საკუთარ loan-ს და Processing სტატუსში
            //    if (loan.UserId != userId)
            //        throw new Exception("You are not allowed to update this loan.");

            //    if (loan.Status != LoanStatus.Processing)
            //        throw new Exception("You can update loan only in 'Processing' status.");
            //}

            // განახლება
            loan.Amount = updateLoanDTO.Amount;
            loan.Currency = updateLoanDTO.Currency;
            loan.PeriodInMonths = updateLoanDTO.PeriodInMonths;
            loan.loanType = updateLoanDTO.LoanType;

            await _context.SaveChangesAsync();

            return loan;
        }


        public async Task<bool> DeleteLoanAsync(int userId, int loanId, Role role)
        {
            var loan = await _context.Loans.FirstOrDefaultAsync(l => l.Id == loanId);

            if (loan == null)
                throw new Exception("Loan not found.");

            if (role != Role.Administrator)
            {
                if (loan.UserId != userId)
                    throw new Exception("You are not allowed to delete this loan.");

                if (loan.Status != LoanStatus.Processing)
                    throw new Exception("You can delete loan only in 'Processing' status.");
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return true;
        }



        public async Task<bool> SetUserBlockStatusAsync(int targetUserId, bool isBlocked, Role role)
        {
            if (role != Role.Administrator)
                throw new Exception("Only admin can block or unblock users.");

            var user = await _context.Users.FindAsync(targetUserId);
            if (user == null)
                throw new Exception("User not found.");

            user.IsBlocked = isBlocked;

            await _context.SaveChangesAsync();
            return true;
        }


    }
}
