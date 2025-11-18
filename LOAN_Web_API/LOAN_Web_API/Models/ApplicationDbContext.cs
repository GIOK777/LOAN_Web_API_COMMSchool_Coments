using LOAN_Web_API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LOAN_Web_API.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }
    }
    
}
