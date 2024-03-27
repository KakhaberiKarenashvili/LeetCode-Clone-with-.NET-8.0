using System.ComponentModel.DataAnnotations;

namespace informaticsge.models;

public class Problem
{
    [Key]
    public int Id { set; get; }
    
    public string Name { set; get; } 
    
    public string problem { set; get; }
    
    public string Tag { set; get; }
    
    public string Difficulty { set; get; }

    public int RuntimeLimit { set; get; }
    
    public int MemoryLimit { set; get; }
    
    public virtual ICollection<TestCase>? TestCases { get; set; }
}