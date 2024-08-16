using informaticsge.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace informaticsge.Entity;

public class AppDbContext : IdentityDbContext<User>
{
    private readonly ILogger<AppDbContext> _logger;
    public AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger) : base(options)
    {
        _logger = logger;
        
        try
        {
            var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (databaseCreator != null)
            {
                if(!databaseCreator.CanConnect()) databaseCreator.Create();
                if(!databaseCreator.HasTables()) databaseCreator.CreateTables();
            }
        }
        catch (Exception exception)
        {
            _logger.LogCritical(exception.Message);
        }
    }
    
    public DbSet<Submissions> Submissions { set; get; }
    public DbSet<Problem> Problems { set; get; }
    public DbSet<TestCase> TestCases { set; get; }
   
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
       
   }
    
}