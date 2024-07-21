using System.ComponentModel.DataAnnotations;

namespace informaticsge.models;

public class TestCase
{
    [Key]
    public int Id { get; set; }

    public string Input { get; set; }

    public string ExpectedOutput { get; set; }
    
    // Foreign key to link to the Problem table
    public int ProblemId { get; set; }

    // Navigation property
    public Problem Problem { get; set; }
}