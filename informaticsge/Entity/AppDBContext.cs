using System.Configuration;
using informaticsge.models;
using informaticsge.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace informaticsge.entity;

public class AppDBContext : IdentityDbContext<User>

{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
        
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