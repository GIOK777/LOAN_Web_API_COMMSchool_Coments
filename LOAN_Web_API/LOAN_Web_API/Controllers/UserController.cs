using LOAN_Web_API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LOAN_Web_API.Controllers
{
    [Authorize(Policy = "UserPolicy")] // წვდომა აქვს User-ს და Admin-ს
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User/me (საკუთარი პროფილის ნახვა)
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            var userId = GetUserId(); // იღებს ID-ს ტოკენიდან

            // -----------------------------------------------------------------------------
            // --- !!! ტესტური Null Reference Exception გამოწვევა !!! ---
            if (userId == 1) // მხოლოდ ადმინისტრატორისთვის ან კონკრეტული ID-ისთვის
            {
                string? testString = null;
                // ვცდილობთ გამოვიძახოთ მეთოდი null ობიექტზე, რაც გამოიწვევს კრახს
                testString!.Trim();
            }
            // -----------------------------------------------------------------------------


            // UserService უკვე დაცულია, რადგან ის მხოლოდ მოცემულ ID-ს ეძებს.
            var result = await _userService.GetUserByIdAsync(userId);

            if (result.IsSuccess)
            {
                // ვაბრუნებთ მომხმარებლის DTO-ს (პაროლის ჰეშის გარეშე)
                return Ok(result.Data);
            }

            // ეს Failure სავარაუდოდ არასოდეს მოხდება, თუ ტოკენი ვალიდურია, 
            // მაგრამ მაინც უსაფრთხოების მიზნით ვტოვებთ.
            return StatusCode(result.StatusCode, new { error = result.ErrorMessage });
        }

        // Admin ფუნქციები (როგორიცაა დაბლოკვა) რჩება AdminController-ში.
    }
}
