using LOAN_Web_API.Interfaces;
using LOAN_Web_API.Models.DTOs;
using LOAN_Web_API.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LOAN_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly ICurrentUserService _currentUser;
        public LoanController(ILoanService loanService, ICurrentUserService currentUser)
        {
            _loanService = loanService;
            _currentUser = currentUser;
        }


        // მომხმარებელი + ადმინი შეუძლია ნახვა
        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetMyLoans()
        {
            var loans = await _loanService.GetUserLoansAsync(_currentUser.UserId);
            return Ok(loans);
        }

        // მხოლოდ მომხმარებელმა შეიძლება შექმნა
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateLoan(CreateLoanDTO createLoanDTO)
        {
            var loan = await _loanService.CreateLoanAsync(_currentUser.UserId, createLoanDTO);
            return Ok(loan);
        }

        // ორივეს შეუძლია განახლება (ადმინს ყველას, იუზერს თავისის)
        [Authorize(Roles = "User,Admin")]
        [HttpPut("{loanId}")]
        public async Task<IActionResult> UpdateLoan(int loanId, LoanRequestDTO updateLoanDTO)
        {
            var loan = await _loanService.UpdateLoanAsync(
                _currentUser.UserId,
                loanId,
                updateLoanDTO,
                _currentUser.Role
            );

            return Ok(loan);
        }

        // მხოლოდ ადმინს შეუძლია მომხმარებლის დაბლოკვა
        [Authorize(Roles = "Admin")]
        [HttpPost("block/{userId}")]
        public async Task<IActionResult> BlockUser(int userId)
        {
            await _loanService.BlockUserAsync(userId);
            return Ok();
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
