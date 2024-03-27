using System.Configuration;
using informaticsge.models;
using informaticsge.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace informaticsge.entity;

public class AppDBcontext : IdentityDbContext<User>

{
    public AppDBcontext(DbContextOptions<AppDBcontext> options) : base(options)
    {
        
    }
    
    public DbSet<Solution> Solutions { set; get; }
    public DbSet<Problem> Problems { set; get; }
    public DbSet<TestCase> TestCases { set; get; }
   
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       base.OnModelCreating(modelBuilder);

       modelBuilder.Entity<Solution>()
           .HasOne(s => s.User)
           .WithMany(u => u.Solutions)
           .HasForeignKey(s => s.UserId);

       modelBuilder.Entity<TestCase>()
           .HasOne(t => t.Problem)
           .WithMany(p => p.TestCases)
           .HasForeignKey(t => t.ProblemId);
       
   }
    
}