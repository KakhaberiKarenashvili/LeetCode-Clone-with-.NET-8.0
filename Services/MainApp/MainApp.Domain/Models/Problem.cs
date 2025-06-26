using System.ComponentModel.DataAnnotations;

namespace MainApp.Domain.Models;

public class Problem
{
    [Key]
    public int Id { set; get; }
    
    public string Name { set; get; } 
    
    public string ProblemText { set; get; }
    
    public string? Tag { set; get; }
    
    public string? Difficulty { set; get; }

    public int RuntimeLimit { set; get; }
    
    public int MemoryLimit { set; get; }
    
    public virtual ICollection<TestCase>? TestCases { get; set; }
}