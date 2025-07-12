namespace MainApp.Application.Dto.Response;

public class GetProblemsResponseDto
{
    public int Id { set; get;  }
    
    public string? Name { set; get; }
    
    public List<string>? Categories { set; get; }
    
    public string? Difficulty { set; get; }
    
}