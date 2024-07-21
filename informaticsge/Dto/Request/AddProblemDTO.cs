namespace informaticsge.Dto;

public class AddProblemDto
{
        public string Name { get; set; }
        
        public string Problem { get; set; }
        
        public string Tag { get; set; }
        
        public string Difficulty { get; set; }

        public int RuntimeLimit { get; set; }
        
        public int MemoryLimit { get; set; }
        
        public ICollection<AddTestCasesDto> TestCases { get; set; }
}