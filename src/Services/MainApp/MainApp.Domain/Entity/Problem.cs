using System.ComponentModel.DataAnnotations;
using BuildingBlocks.Common.Enums;

namespace MainApp.Domain.Entity;

public class Problem
{
    [Key]
    public int Id { set; get; }
    
    public string Name { set; get; } 
    
    public string ProblemText { set; get; }
    
    public Difficulty Difficulty { set; get; }
    
    public Category Category { set; get; }
    
    public int RuntimeLimit { set; get; }
    
    public int MemoryLimit { set; get; }
    public virtual ICollection<TestCase>? TestCases { get; set; }
}