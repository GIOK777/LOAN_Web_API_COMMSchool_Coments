using LOAN_Web_API.Interfaces;
using LOAN_Web_API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LOAN_Web_API.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly ILoanService _loanService;

        public AdminController(IUserService userService, ILoanService loanService)
        {
            _userService = userService;
            _loanService = loanService;
        }



        // --- USER MANAGEMENT ---

        // PUT: api/Admin/user/{userId}/block
        [HttpPut("user/{userId}/block")]
        public async Task<IActionResult> BlockUser(int userId, [FromQuery] bool isBlocked = true)
        {
            var result = await _userService.BlockUserAsync(userId, isBlocked);

            if (result.IsSuccess)
            {
                return NoContent(); // 204 No Content
            }

            // 404 Not Found ან 500 Server Error
            return StatusCode(result.StatusCode, new { error = result.ErrorMessage });
        }






        // --- LOAN MANAGEMENT ---

        // GET: api/Admin/loans (ყველა სესხი)
        [HttpGet("loans")]
        public async Task<IActionResult> GetAllLoans()
        {
            var result = await _loanService.GetAllLoansAsync();

            return StatusCode(result.StatusCode, result.Data);
        }




        // PUT: api/Admin/loan/{loanId}/status (სტატუსის შეცვლა)
        [HttpPut("loan/{loanId}/status")]
        public async Task<IActionResult> UpdateLoanStatus(int loanId, [FromBody] LoanStatusUpdateDTO loanStatusUpdateDTO)
        {
            // აქაც დაგვჭირდება LoanStatusUpdateDto-ის ვალიდატორი
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _loanService.UpdateLoanStatusAsync(loanId, loanStatusUpdateDTO);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Data);
            }

            return StatusCode(result.StatusCode, new { error = result.ErrorMessage });
        }




        // DELETE: api/Admin/loan/{loanId} (სესხის წაშლა სტატუსის მიუხედავად)
        [HttpDelete("loan/{loanId}")]
        public async Task<IActionResult> DeleteLoanAdmin(int loanId)
        {
            var result = await _loanService.DeleteLoanAdminAsync(loanId);

            if (result.IsSuccess)
            {
                return NoContent(); // 204 No Content
            }

            return StatusCode(result.StatusCode, new { error = result.ErrorMessage });
        }
    }
}
