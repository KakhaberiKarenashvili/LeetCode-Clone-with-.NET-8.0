using System.ComponentModel.DataAnnotations;
using BuildingBlocks.Common.Dtos;
using BuildingBlocks.Common.Helpers;
using MainApp.Domain.Entity;

namespace MainApp.Application.Dto.Request;

public class AddProblemDto
{ 
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        [Required]
        public string ProblemText { get; set; }
        
        [Required]
        public List<string> Categories { get; set; }
        
        public string Difficulty { get; set; }
        
        [Required]
        public int RuntimeLimit { get; set; }
       
        [Required]
        public int MemoryLimit { get; set; }
        
        public ICollection<TestCaseDto>? TestCases { get; set; }

        public static Problem ToProblem(AddProblemDto problemDto)
        {
                return new Problem
                {
                        Name = problemDto.Name,
                        ProblemText = problemDto.ProblemText,
                        Category = EnumParser.ParseCategoriesFromStrings(problemDto.Categories),
                        Difficulty = EnumParser.ParseDifficultyFromString(problemDto.Difficulty),
                        RuntimeLimit = problemDto.RuntimeLimit,
                        MemoryLimit = problemDto.MemoryLimit,
                        TestCases = problemDto.TestCases.Select(tc => new TestCase
                        {
                                Input = tc.Input,
                                ExpectedOutput = tc.ExpectedOutput
                        }).ToList()
                };
        }
}