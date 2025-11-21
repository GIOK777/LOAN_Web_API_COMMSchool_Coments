using LOAN_Web_API.Interfaces;
using LOAN_Web_API.Models.DTOs;
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

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        [HttpPost]
        public async Task<IActionResult> CreateLoan(CreateLoanDTO createLoanDTO)
        {
            var userId = GetUserId();
            var loan = await _loanService.CreateLoanAsync(userId, createLoanDTO);
            return Ok(loan);
        }

        [HttpGet]
        public async Task<IActionResult> GetLoans()
        {
            var userId = GetUserId();
            var loans = await _loanService.GetUserLoansAsync(userId);
            return Ok(loans);
        }

        [HttpPut("{loanId}")]
        public async Task<IActionResult> UpdateLoan(int loanId, UpdateLoanDto updateLoanDto)
        {
            var userId = GetUserId();
            var loan = await _loanService.UpdateLoanAsync(userId, loanId, updateLoanDto);
            return Ok(loan);
        }

        [HttpDelete("{loanId}")]
        public async Task<IActionResult> DeleteLoan(int loanId)
        {
            var userId = GetUserId();
            await _loanService.DeleteLoanAsync(userId, loanId);
            return Ok("Deleted");
        }
    }
}
