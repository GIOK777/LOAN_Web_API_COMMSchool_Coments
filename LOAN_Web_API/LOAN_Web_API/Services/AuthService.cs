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


        public async Task<CustomResult<UserResponseDTO>> RegisterAsync(UserRegisterDTO userRegisterDto)
        {
            // 1. უნიკალურობა
            if (await _context.Users.AnyAsync(u => u.UserName == userRegisterDto.UserName || u.Email == userRegisterDto.Email))
            {
                return CustomResult<UserResponseDTO>.Failure("Username or Email already exists.", 409); // Conflict
            }

            // 2. DTO -> Entity
            var user = _mapper.Map<User>(userRegisterDto);

            // 3. Hashing
            user.PasswordHash = PasswordHasher.HashPassword(userRegisterDto.Password);
            user.Role = Role.User;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // 4. დაბრუნება
            var userDto = _mapper.Map<UserResponseDTO>(user);
            // სტატუს კოდი 201 (Created)
            return CustomResult<UserResponseDTO>.Success(userDto, 201);
        }

        //public async Task<User> RegisterAsync(UserRegisterDTO userRegisterDTO)
        //{
        //    // 1. უნიკალურობის შემოწმება (თუმცა Fluent Validation-მაც უნდა შეამოწმოს)
        //    if (await _context.Users.AnyAsync(u => u.UserName == userRegisterDTO.UserName))
        //    {
        //        // ჩვენი CustomResult-ის გამო, აქ Exception-ს არ ვისვრით, მაგრამ ამ სერვისიდან 
        //        // კონტროლერში გადასატანად, ამ ეტაპზე შეგვიძლია Exception-ის გამოყენება ან CustomResult-ის დაბრუნება.
        //        // რადგან IAuthService Task<User> აბრუნებს, დროებით Exception-ს ვისვრით. 
        //        // უკეთესი იქნება თუ IAuthService Task<CustomResult<User>> დააბრუნებს. 
        //        // მოდით, შევცვალოთ IAuthService უკეთესი პრაქტიკისთვის.

        //        throw new ApplicationException("Username already exists.");
        //    }


        //    // 2. DTO-დან Entity-ში გარდაქმნა
        //    var user = _mapper.Map<User>(userRegisterDTO);

        //    // 3. პაროლის დაჰეშვა
        //    user.PasswordHash = PasswordHasher.HashPassword(userRegisterDTO.Password);
        //    user.Role = Role.User; // ნაგულისხმევად მომხმარებელია
        //    user.IsBlocked = false;

        //    // 4. ბაზაში შენახვა
        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();

        //    return user;
        //}


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

        //public async Task<string> LoginAsync(UserLoginDTO userLoginDTO)
        //{
        //    // 1. მომხმარებლის მოძიება UserName-ით
        //    var user = await _context.Users
        //        .FirstOrDefaultAsync(u => u.UserName == userLoginDTO.UserName);

        //    if (user == null)
        //    {
        //        throw new UnauthorizedAccessException("Invalid username or password.");
        //    }

        //    // 2. პაროლის შემოწმება
        //    if (!PasswordHasher.VerifyPassword(userLoginDTO.Password, user.PasswordHash))
        //    {
        //        throw new UnauthorizedAccessException("Invalid username or password.");
        //    }

        //    // 3. JWT ტოკენის გენერაცია
        //    var token = _jwtGenerator.GenerateToken(user);

        //    return token;
        //}




        //-----------------------------------------------------------------------------
        //private string GenerateJwtToken(User user)
        //{
        //    var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //    new Claim(ClaimTypes.Name, user.UserName),
        //    new Claim(ClaimTypes.Role, user.Role.ToString())
        //};
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
        //        _config["Jwt:Key"]
        //    ));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    var token = new JwtSecurityToken(
        //        issuer: _config["Jwt:Issuer"],
        //        audience: _config["Jwt:Audience"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddHours(2),
        //        signingCredentials: creds
        //    );
        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
    }
}
