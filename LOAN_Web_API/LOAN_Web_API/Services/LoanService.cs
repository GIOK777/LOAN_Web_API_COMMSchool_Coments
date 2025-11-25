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
        public async Task<CustomResult<LoanResponseDTO>> AddLoanAsync(int userId, LoanRequestDTO loanRequestDTO)
        {
            // 1. შემოწმება: თუ მომხმარებელი დაბლოკილია, სესხის აღების უფლება არ აქვს.
            if (await IsUserBlockedAsync(userId))
            {
                return CustomResult<LoanResponseDTO>.Failure(
                    "Loan request rejected: User is blocked and cannot apply for a loan.", 403); // Forbidden
            }

            // 2. DTO -> Entity გარდაქმნა და ველების დაყენება
            var loan = _mapper.Map<Loan>(loanRequestDTO);
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
        public async Task<CustomResult<LoanResponseDTO>> UpdateLoanAsync(int userId, int loanId, LoanUpdateDTO loanUpdateDTO)
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
            _mapper.Map(loanUpdateDTO, loan);

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
    }
}
