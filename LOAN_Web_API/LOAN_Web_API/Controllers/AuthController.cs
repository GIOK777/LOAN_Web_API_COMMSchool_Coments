using LOAN_Web_API.Interfaces;
using LOAN_Web_API.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LOAN_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO registerDTO)
        {
            var result = await _auth.RegisterAsync(registerDTO);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO loginDTO)
        {
            var token = await _auth.LoginAsync(loginDTO);
            return Ok(new { token });
        }
    }
}
