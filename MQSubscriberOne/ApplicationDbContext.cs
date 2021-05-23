using Microsoft.EntityFrameworkCore;
using MQSubscriberOne.Models;

namespace MQSubscriberOne
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {

            }

        public DbSet<Message> Messages { get; set; }
    }
}
