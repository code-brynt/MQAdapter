using Microsoft.EntityFrameworkCore;
using MQSubscriberTwo.Models;

namespace MQSubscriberTwo
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public DbSet<Message> Messages { get; set; }
    }
}
