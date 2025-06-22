using System.ComponentModel.DataAnnotations;

namespace MainApp.Domain.Models;

public class TestCase
{
    [Key]
    public int Id { get; set; }

    public string Input { get; set; }

    public string ExpectedOutput { get; set; }
    
    public int ProblemId { get; set; }
    
    public Problem Problem { get; set; }
}