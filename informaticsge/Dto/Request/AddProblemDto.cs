using System.ComponentModel.DataAnnotations;
using informaticsge.models;

namespace informaticsge.Dto.Request;

public class AddProblemDto
{ 
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        [Required]
        public string ProblemText { get; set; }
        
        [Required]
        public string Tag { get; set; }
        
        public string Difficulty { get; set; }
        
        [Required]
        public int RuntimeLimit { get; set; }
       
        [Required]
        public int MemoryLimit { get; set; }
        
        public ICollection<TestCaseDto> TestCases { get; set; }
}