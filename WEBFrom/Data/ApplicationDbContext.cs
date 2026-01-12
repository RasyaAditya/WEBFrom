using Microsoft.EntityFrameworkCore;
using WEBFrom.Models;

namespace WEBFrom.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

       
        public DbSet<EmployeeRequest> EmployeeRequests { get; set; }
    }
}
