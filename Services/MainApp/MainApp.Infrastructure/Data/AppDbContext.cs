using MainApp.Domain.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public DbSet<Submissions> Submissions { set; get; }
    public DbSet<Problem> Problems { set; get; }
    public DbSet<TestCase> TestCases { set; get; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       base.OnModelCreating(modelBuilder);

       modelBuilder.Entity<Submissions>()
           .HasOne(s => s.User)
           .WithMany(u => u.Submissions)
           .HasForeignKey(s => s.UserId);

       modelBuilder.Entity<TestCase>()
           .HasOne(t => t.Problem)
           .WithMany(p => p.TestCases)
           .HasForeignKey(t => t.ProblemId);
       
       modelBuilder.Entity<Problem>()
           .Property(p => p.Difficulty)
           .HasConversion<int>();
       
   }
    
}