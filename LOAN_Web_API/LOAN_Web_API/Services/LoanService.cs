using AutoMapper;
using LOAN_Web_API.Interfaces;
using LOAN_Web_API.Models;
using LOAN_Web_API.Models.DTOs;
using LOAN_Web_API.Models.Entities;
using LOAN_Web_API.Models.Enums;
using LOAN_Web_API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LOAN_Web_API.Services
{
    public class LoanService : ILoanService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LoanService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        // --- დამხმარე მეთოდი: მომხმარებლის დაბლოკილობის შემოწმება ---
        private async Task<bool> IsUserBlockedAsync(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Select(u => new { u.Id, u.IsBlocked })
                .FirstOrDefaultAsync(u => u.Id == userId);

            // თუ მომხმარებელი არ არსებობს, მაინც დაბლოკილად ჩავთვალოთ სესხის აღებისას
            return user?.IsBlocked ?? true;
        }


        //                           USER ROLE METHODS
        //------------------------------------------------------------------------------------

        // --- 1. სესხის დამატება (AddLoanAsync) ---
        public async Task<CustomResult<LoanResponseDTO>> AddLoanAsync(int userId, LoanRequestDTO LoanRequestDTO)
        {
            // 1. შემოწმება: თუ მომხმარებელი დაბლოკილია, სესხის აღების უფლება არ აქვს.
            if (await IsUserBlockedAsync(userId))
            {
                return CustomResult<LoanResponseDTO>.Failure(
                    "Loan request rejected: User is blocked and cannot apply for a loan.", 403); // Forbidden
            }

            // 2. DTO -> Entity გარდაქმნა და ველების დაყენება
            var loan = _mapper.Map<Loan>(LoanRequestDTO);
            loan.UserId = userId;
            loan.Status = LoanStatus.Processing; // სტატუსი default-ად არის "დამუშავების პროცესში"

            // 3. შენახვა
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            var loanDto = _mapper.Map<LoanResponseDTO>(loan);
            return CustomResult<LoanResponseDTO>.Success(loanDto, 201); // Created
        }



        // --- 2. საკუთარი სესხების დათვალიერება (GetUserLoansAsync) ---
        public async Task<CustomResult<IEnumerable<LoanResponseDTO>>> GetUserLoansAsync(int userId)
        {
            var loans = await _context.Loans
                .AsNoTracking()
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.Id)
                .ToListAsync();

            var loanDtos = _mapper.Map<IEnumerable<LoanResponseDTO>>(loans);
            return CustomResult<IEnumerable<LoanResponseDTO>>.Success(loanDtos);
        }


        // --- 3. სესხის განახლება (UpdateLoanAsync) ---
        public async Task<CustomResult<LoanResponseDTO>> UpdateLoanAsync(int userId, int loanId, LoanUpdateDTO LoanUpdateDTO)
        {
            var loan = await _context.Loans
                .FirstOrDefaultAsync(l => l.Id == loanId && l.UserId == userId); // ამოწმებს საკუთრებას

            if (loan == null)
            {
                return CustomResult<LoanResponseDTO>.Failure("Loan not found or unauthorized access.", 404);
            }

            // შემოწმება: განახლება და წაშლა მხოლოდ 'Processing' სტატუსით შეიძლება
            if (loan.Status != LoanStatus.Processing)
            {
                return CustomResult<LoanResponseDTO>.Failure(
                    "Cannot update: Loan status is not 'Processing'.", 403);
            }

            // DTO -> Entity გარდაქმნა (განახლება)
            _mapper.Map(LoanUpdateDTO, loan);

            await _context.SaveChangesAsync();

            var loanDto = _mapper.Map<LoanResponseDTO>(loan);
            return CustomResult<LoanResponseDTO>.Success(loanDto);
        }



        // --- 4. სესხის წაშლა (DeleteLoanAsync) ---
        public async Task<CustomResult<bool>> DeleteLoanAsync(int userId, int loanId)
        {
            var loan = await _context.Loans
                .FirstOrDefaultAsync(l => l.Id == loanId && l.UserId == userId); // ამოწმებს საკუთრებას

            if (loan == null)
            {
                return CustomResult<bool>.Failure("Loan not found or unauthorized access.", 404);
            }

            // შემოწმება: წაშლა მხოლოდ 'Processing' სტატუსით შეიძლება
            if (loan.Status != LoanStatus.Processing)
            {
                return CustomResult<bool>.Failure(
                    "Cannot delete: Loan status is not 'Processing'.", 403);
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return CustomResult<bool>.Success(true, 204); // No Content
        }




        //                           ADMIN ROLE METHODS
        // ------------------------------------------------------------------

        // --- 5. ყველა სესხის ნახვა (GetAllLoansAsync) ---
        public async Task<CustomResult<IEnumerable<LoanResponseDTO>>> GetAllLoansAsync()
        {
            // ადმინისტრატორს შეუძლია ნებისმიერი მომხმარებლის სესხის ნახვა
            var loans = await _context.Loans
                .AsNoTracking()
                .OrderByDescending(l => l.Id)
                .ToListAsync();

            var loanDtos = _mapper.Map<IEnumerable<LoanResponseDTO>>(loans);
            return CustomResult<IEnumerable<LoanResponseDTO>>.Success(loanDtos);
        }


        // --- 6. სესხის სტატუსის განახლება (UpdateLoanStatusAsync) ---
        // (ამას მხოლოდ ადმინისტრატორი აკეთებს)
        public async Task<CustomResult<LoanResponseDTO>> UpdateLoanStatusAsync(int loanId, LoanStatusUpdateDTO loanStatusUpdateDTO)
        {
            var loan = await _context.Loans
                .FirstOrDefaultAsync(l => l.Id == loanId);

            if (loan == null)
            {
                return CustomResult<LoanResponseDTO>.Failure("Loan not found.", 404);
            }

            // სტატუსის ცვლილება
            loan.Status = loanStatusUpdateDTO.Status;

            await _context.SaveChangesAsync();

            var loanDto = _mapper.Map<LoanResponseDTO>(loan);
            return CustomResult<LoanResponseDTO>.Success(loanDto);
        }



        // --- 7. სესხის წაშლა ადმინისტრატორის მიერ (DeleteLoanAdminAsync) ---
        // (სტატუსს მნიშვნელობა არ აქვს)
        public async Task<CustomResult<bool>> DeleteLoanAdminAsync(int loanId)
        {
            var loan = await _context.Loans
                .FirstOrDefaultAsync(l => l.Id == loanId);

            if (loan == null)
            {
                return CustomResult<bool>.Failure("Loan not found.", 404);
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return CustomResult<bool>.Success(true, 204); // No Content
        }




        //    public async Task<Loan> CreateLoanAsync(int userId, CreateLoanDTO createLoanDTO, Role role)
        //    {
        //        // Admin bypass  
        //        if (role != Role.Administrator)
        //        {
        //            var user = await _context.Users.FindAsync(userId);
        //            if (user.IsBlocked)
        //                throw new Exception("You are blocked and cannot create a loan.");
        //        }

        //        var loan = new Loan
        //        {
        //            UserId = userId,
        //            Amount = createLoanDTO.Amount,
        //            Currency = createLoanDTO.Currency,
        //            PeriodInMonths = createLoanDTO.PeriodInMonths,
        //            loanType = createLoanDTO.LoanType,
        //            Status = LoanStatus.Processing,
        //        };

        //        _context.Loans.Add(loan);
        //        await _context.SaveChangesAsync();
        //        return loan;
        //    }



        //    // როცა უბრალოდ "ვბრუნებ კოლექციას" და nothing more -  ვიყენებთ IEnumerable
        //    public async Task<IEnumerable<Loan>> GetLoansAsync(int userId, Role role)
        //    {
        //        if (role == Role.Administrator)
        //            return await _context.Loans.ToListAsync();

        //        return await _context.Loans
        //            .Where(l => l.UserId == userId)
        //            .ToListAsync();
        //    }


        //    public async Task<Loan> UpdateLoanAsync(int currentUserId, int loanId, LoanRequestDTO updateLoanDTO, Role role)
        //    {
        //        var loan = await _context.Loans.FindAsync(loanId);

        //        if (loan == null)
        //            throw new Exception("Loan not found.");


        //        // თუ არაა ადმინი და ცდილობს სხვის სესხს
        //        if (role == Role.User && loan.UserId != currentUserId)
        //            throw new Exception("Forbidden");


        //        //// Admin → შეუძლია განახლება ყველას loan-ზე
        //        //if (role != Role.Administrator)
        //        //{
        //        //    // ოღონდ უბრალო User → მხოლოდ საკუთარ loan-ს და Processing სტატუსში
        //        //    if (loan.UserId != userId)
        //        //        throw new Exception("You are not allowed to update this loan.");

        //        //    if (loan.Status != LoanStatus.Processing)
        //        //        throw new Exception("You can update loan only in 'Processing' status.");
        //        //}

        //        // განახლება
        //        loan.Amount = updateLoanDTO.Amount;
        //        loan.Currency = updateLoanDTO.Currency;
        //        loan.PeriodInMonths = updateLoanDTO.PeriodInMonths;
        //        loan.loanType = updateLoanDTO.LoanType;

        //        await _context.SaveChangesAsync();

        //        return loan;
        //    }


        //    public async Task<bool> DeleteLoanAsync(int userId, int loanId, Role role)
        //    {
        //        var loan = await _context.Loans.FirstOrDefaultAsync(l => l.Id == loanId);

        //        if (loan == null)
        //            throw new Exception("Loan not found.");

        //        if (role != Role.Administrator)
        //        {
        //            if (loan.UserId != userId)
        //                throw new Exception("You are not allowed to delete this loan.");

        //            if (loan.Status != LoanStatus.Processing)
        //                throw new Exception("You can delete loan only in 'Processing' status.");
        //        }

        //        _context.Loans.Remove(loan);
        //        await _context.SaveChangesAsync();

        //        return true;
        //    }



        //    public async Task<bool> SetUserBlockStatusAsync(int targetUserId, bool isBlocked, Role role)
        //    {
        //        if (role != Role.Administrator)
        //            throw new Exception("Only admin can block or unblock users.");

        //        var user = await _context.Users.FindAsync(targetUserId);
        //        if (user == null)
        //            throw new Exception("User not found.");

        //        user.IsBlocked = isBlocked;

        //        await _context.SaveChangesAsync();
        //        return true;
        //    }


    }
}
