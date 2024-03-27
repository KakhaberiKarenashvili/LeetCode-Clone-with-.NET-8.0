using System.ComponentModel.DataAnnotations;

namespace informaticsge.Dto;

public class GetProblemsDTO
{
    public int Id { set; get;  }
    
    public string Name { set; get; }
    
    public string Tag { set; get; }
}