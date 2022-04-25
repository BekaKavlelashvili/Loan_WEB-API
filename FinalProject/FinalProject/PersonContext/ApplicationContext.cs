using FinalProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.PersonContext
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
        }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
        public DbSet<Loan> loans { get; set; }
        public DbSet<Users> users { get; set; }
        public DbSet<Accountant> accountants { get; set; }
    }
}
