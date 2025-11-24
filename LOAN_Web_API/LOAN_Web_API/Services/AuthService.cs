using AutoMapper;
using LOAN_Web_API.Interfaces;
using LOAN_Web_API.Models;
using LOAN_Web_API.Models.DTOs;
using LOAN_Web_API.Models.Entities;
using LOAN_Web_API.Models.Enums;
using LOAN_Web_API.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LOAN_Web_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IJwtGenerator _jwtGenerator;

        public AuthService(ApplicationDbContext context, IMapper mapper, IJwtGenerator jwtGenerator)
        {
            _context = context;
            _mapper = mapper;
            _jwtGenerator = jwtGenerator;
        }


        public async Task<CustomResult<UserResponseDTO>> RegisterAsync(UserRegisterDTO userRegisterDTO)
        {
            // 1. უნიკალურობა
            if (await _context.Users.AnyAsync(u => u.UserName == userRegisterDTO.UserName || u.Email == userRegisterDTO.Email))
            {
                return CustomResult<UserResponseDTO>.Failure("Username or Email already exists.", 409); // Conflict
            }

            // 2. DTO -> Entity
            var user = _mapper.Map<User>(userRegisterDTO);

            // 3. Hashing
            user.PasswordHash = PasswordHasher.HashPassword(userRegisterDTO.Password);
            user.Role = Role.User;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // 4. დაბრუნება
            var userDto = _mapper.Map<UserResponseDTO>(user);
            // სტატუს კოდი 201 (Created)
            return CustomResult<UserResponseDTO>.Success(userDto, 201);
        }



     

        public async Task<CustomResult<string>> LoginAsync(UserLoginDTO userLoginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userLoginDto.UserName);

            if (user == null || !PasswordHasher.VerifyPassword(userLoginDto.Password, user.PasswordHash))
            {
                return CustomResult<string>.Failure("Invalid credentials.", 401); // Unauthorized
            }

            // JWT ტოკენის გენერაცია
            var token = _jwtGenerator.GenerateToken(user);

            return CustomResult<string>.Success(token);
        }        
    }
}
