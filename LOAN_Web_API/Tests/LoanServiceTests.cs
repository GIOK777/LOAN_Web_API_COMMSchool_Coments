using AutoMapper;
using LOAN_Web_API.Models;
using LOAN_Web_API.Models.DTOs;
using LOAN_Web_API.Models.Entities;
using LOAN_Web_API.Models.Enums;
using LOAN_Web_API.Services;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class LoanServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<IMapper> _mockMapper;
        private readonly LoanService _loanService;
        public LoanServiceTests()
        {
            // 1. Mock-ების ინიციალიზაცია
            _mockContext = new Mock<ApplicationDbContext>();
            _mockMapper = new Mock<IMapper>();

            // 2. LoanService-ის შექმნა Mock-ების გამოყენებით
            _loanService = new LoanService(_mockContext.Object, _mockMapper.Object);
        }

        // -------------------------------------------------------------------
        // ტესტი : სესხის წაშლა (წარუმატებლობა: სხვა მომხმარებლის სესხი)
        // -------------------------------------------------------------------
        [Fact]
        public async Task DeleteLoanAsync_DifferentUserLoan_ShouldReturnFailure()
        {
            // Arrange
            var requestingUserId = 2; // ეს მომხმარებელი ითხოვს წაშლას
            var loanOwnerId = 1;
            var loanId = 12;
            var loan = new Loan
            {
                Id = loanId,
                UserId = loanOwnerId, // სხვისი სესხი
                Status = LoanStatus.Processing,
                Amount = 1000
            };

            // Mock Loans ცხრილი
            var mockLoansDbSet = new List<Loan> { loan }.AsQueryable();
            _mockContext.Setup(c => c.Loans).ReturnsDbSet(mockLoansDbSet);

            // Act
            // ვამოწმებთ, რომ LoanService-ის Find-მა ვერ იპოვა სესხი: l.Id == loanId && l.UserId == requestingUserId
            var result = await _loanService.DeleteLoanAsync(requestingUserId, loanId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode); // 404 Not Found (ან Unauthorized Access)
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Never);
        }
    }
}
