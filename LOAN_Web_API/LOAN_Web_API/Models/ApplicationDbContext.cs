using Microsoft.EntityFrameworkCore;

namespace LOAN_Web_API.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
    }
    
}
