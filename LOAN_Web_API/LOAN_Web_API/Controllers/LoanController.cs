using LOAN_Web_API.Interfaces;
using LOAN_Web_API.Models.DTOs;
using LOAN_Web_API.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LOAN_Web_API.Controllers
{
    [Authorize(Policy = "UserPolicy")] // წვდომა აქვს User-ს და Admin-ს
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : BaseApiController
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }


        // POST: api/Loan
        [HttpPost]
        public async Task<IActionResult> AddLoan([FromBody] LoanRequestDTO loanRequestDTO)
        {
            // აქაც დაგვჭირდება LoanRequestDto-ის ვალიდატორი (დაამატებთ Validations ფოლდერში)
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _loanService.AddLoanAsync(userId, loanRequestDTO);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Data);
            }
            // Failure-ს შემთხვევაში, სტატუსი იქნება 403 (Blocked) ან 400 (Validation Error)
            return StatusCode(result.StatusCode, new { error = result.ErrorMessage });
        }




        // GET: api/Loan/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyLoans()
        {
            var userId = GetUserId();
            var result = await _loanService.GetUserLoansAsync(userId);

            return StatusCode(result.StatusCode, result.Data); // 200 OK
        }




        // PUT: api/Loan/{loanId}
        [HttpPut("{loanId}")]
        public async Task<IActionResult> UpdateLoan(int loanId, [FromBody] LoanUpdateDTO loanUpdateDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _loanService.UpdateLoanAsync(userId, loanId, loanUpdateDTO);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Data);
            }

            // 404 (Not Found), 403 (Not Processing)
            return StatusCode(result.StatusCode, new { error = result.ErrorMessage });
        }



        // DELETE: api/Loan/{loanId}
        [HttpDelete("{loanId}")]
        public async Task<IActionResult> DeleteLoan(int loanId)
        {
            var userId = GetUserId();
            var result = await _loanService.DeleteLoanAsync(userId, loanId);

            if (result.IsSuccess)
            {
                return NoContent(); // 204 No Content
            }

            // 404 (Not Found), 403 (Not Processing)
            return StatusCode(result.StatusCode, new { error = result.ErrorMessage });
        }



        // -----------------------------------------------------------------

        //private int GetUserId() =>
        //    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        //[HttpPost]
        //public async Task<IActionResult> CreateLoan(CreateLoanDTO createLoanDTO)
        //{
        //    var userId = GetUserId();
        //    var loan = await _loanService.CreateLoanAsync(userId, createLoanDTO, Role.);
        //    return Ok(loan);
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetLoans()
        //{
        //    var userId = GetUserId();
        //    var loans = await _loanService.GetUserLoansAsync(userId);
        //    return Ok(loans);
        //}

        //[HttpPut("{loanId}")]
        //public async Task<IActionResult> UpdateLoan(int loanId, UpdateLoanDto updateLoanDto)
        //{
        //    var userId = GetUserId();
        //    var loan = await _loanService.UpdateLoanAsync(userId, loanId, updateLoanDto);
        //    return Ok(loan);
        //}

        //[HttpDelete("{loanId}")]
        //public async Task<IActionResult> DeleteLoan(int loanId)
        //{
        //    var userId = GetUserId();
        //    await _loanService.DeleteLoanAsync(userId, loanId);
        //    return Ok("Deleted");
        //}

        // --------------------------------------------------------

        // სესხის შექმნა — მხოლოდ User-ს, Admin-ს არა
        //[HttpPost]
        //[Authorize(Roles = "User")]
        //public async Task<IActionResult> CreateLoan(CreateLoanDTO createLoanDTO)
        //{
        //    var result = await _loanService.CreateLoanAsync(User, createLoanDTO);
        //    return Ok(result);
        //}

        //// საკუთარი სესხების ნახვა — User & Admin
        //[HttpGet]
        //[Authorize(Roles = "User,Administrator")]
        //public async Task<IActionResult> GetLoans()
        //{
        //    var result = await _loanService.GetUserLoansAsync(User);
        //    return Ok(result);
        //}

        //// სესხის განახლება — User & Admin
        //[HttpPut("{loanId}")]
        //[Authorize(Roles = "User,Administrator")]
        //public async Task<IActionResult> UpdateLoan(int loanId, LoanRequestDTO updateLoanDTO)
        //{
        //    var result = await _loanService.UpdateLoanAsync(User, loanId, updateLoanDTO);
        //    return Ok(result);
        //}

        //// სესხის წაშლა — User & Admin
        //[HttpDelete("{loanId}")]
        //[Authorize(Roles = "User,Administrator")]
        //public async Task<IActionResult> DeleteLoan(int loanId)
        //{
        //    var result = await _loanService.DeleteLoanAsync(User, loanId);
        //    return Ok(result);
        //}

        //// Admin — ყველა მომხმარებლის სესხების ნახვა
        //[HttpGet("all")]
        //[Authorize(Roles = "Administrator")]
        //public async Task<IActionResult> GetAllLoans()
        //{
        //    var result = await _loanService.GetAllLoansAsync();
        //    return Ok(result);
        //}

        //// Admin — მომხმარებლის დაბლოკვა
        //[HttpPut("block/{userId}")]
        //[Authorize(Roles = "Administrator")]
        //public async Task<IActionResult> BlockUser(int userId)
        //{
        //    await _loanService.BlockUserAsync(userId);
        //    return Ok("User blocked");
        //}
    }
}
