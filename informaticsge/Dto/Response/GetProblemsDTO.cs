using System.ComponentModel.DataAnnotations;

namespace informaticsge.Dto;

public class GetProblemsDto
{
    public int Id { set; get;  }
    
    public string Name { set; get; }
    
    public string Tag { set; get; }
}