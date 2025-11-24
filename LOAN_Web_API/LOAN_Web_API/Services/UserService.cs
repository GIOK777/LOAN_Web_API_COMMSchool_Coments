using AutoMapper;
using LOAN_Web_API.Interfaces;
using LOAN_Web_API.Models;
using LOAN_Web_API.Models.DTOs;
using LOAN_Web_API.Utilities;
using Microsoft.EntityFrameworkCore;
using System;

namespace LOAN_Web_API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // --- 1. მომხმარებლის მიღება ID-ის მიხედვით (GetUserByIdAsync) ---
        // გამოიყენება როგორც User-ისთვის (თავისი პროფილის სანახავად), ისე Admin-ისთვის.
        public async Task<CustomResult<UserResponseDTO>> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .AsNoTracking() // მონაცემების წაკითხვის ოპტიმიზაცია
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                // თუ მომხმარებელი არ მოიძებნა, ვაბრუნებთ Failure-ს 404 სტატუს კოდით
                return CustomResult<UserResponseDTO>.Failure($"User with ID {id} not found.", 404);
            }

            // Entity-ს გარდაქმნა DTO-დ
            var userDto = _mapper.Map<UserResponseDTO>(user);

            return CustomResult<UserResponseDTO>.Success(userDto);
        }

        // --- 2. მომხმარებლის დაბლოკვა/განბლოკვა (BlockUserAsync) - მხოლოდ Admin ფუნქცია ---
        public async Task<CustomResult<bool>> BlockUserAsync(int userId, bool isBlocked)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                // თუ მომხმარებელი არ მოიძებნა
                return CustomResult<bool>.Failure($"User with ID {userId} not found.", 404);
            }

            // ცვლილების ლოგიკა
            user.IsBlocked = isBlocked;

            // ბაზაში შენახვა
            try
            {
                await _context.SaveChangesAsync();

                // ვაბრუნებთ true-ს, რაც ოპერაციის წარმატებაზე მიუთითებს
                return CustomResult<bool>.Success(true);
            }
            catch (DbUpdateException ex)
            {
                // თუ ბაზაში შენახვისას მოხდა შეცდომა
                // აქ კარგი იქნება ლოგირების დამატება
                return CustomResult<bool>.Failure("Failed to update user status due to a database error.", 500);
            }
        }
    }
}
