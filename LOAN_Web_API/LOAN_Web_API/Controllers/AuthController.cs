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
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userRegisterDTO)
        {
            // Fluent Validation-ი ავტომატურად იმუშავებს. თუ არასწორია, API დააბრუნებს 400 Bad Request.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(userRegisterDTO);

            if (result.IsSuccess)
            {
                // თუ წარმატებულია, ვაბრუნებთ 201 Created და მომხმარებლის DTO-ს
                return StatusCode(result.StatusCode, result.Data);
            }

            // თუ წარუმატებელია (მაგ., Username უკვე არსებობს), ვაბრუნებთ შესაბამის შეცდომას
            return StatusCode(result.StatusCode, new { error = result.ErrorMessage });
        }





        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLoginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(userLoginDTO);

            if (result.IsSuccess)
            {
                // თუ წარმატებულია, ვაბრუნებთ 200 OK და JWT ტოკენს
                // ტოკენი წარმოდგენილია როგორც string
                return Ok(new { token = result.Data });
            }

            // თუ წარუმატებელია (არასწორი credentials), ვაბრუნებთ 401 Unauthorized
            return StatusCode(result.StatusCode, new { error = result.ErrorMessage });
        }
    }
}
