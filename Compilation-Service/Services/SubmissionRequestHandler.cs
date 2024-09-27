using Compilation_Service.Dto.Request;
using Compilation_Service.Dto.Response;
using Compilation_Service.RabbitMQ;

namespace Compilation_Service.Services;

public class SubmissionRequestHandler
{
    private readonly PythonTestingService _pythonTestingService;
    private readonly CppTestingService _cppTestingService;
    private readonly RabbitMqService _rabbitMqService;
    private readonly ILogger<SubmissionRequestHandler> _logger;

    public SubmissionRequestHandler(PythonTestingService pythonTestingService, CppTestingService cppTestingService, RabbitMqService rabbitMqService, ILogger<SubmissionRequestHandler> logger)
    {
        _pythonTestingService = pythonTestingService;
        _cppTestingService = cppTestingService;
        _rabbitMqService = rabbitMqService;
        _logger = logger;
    }

    public async Task ProcessSubmissionRequest(SubmissionRequestDto submissionRequestDto)
    {
        _logger.LogInformation("Received Submission Request & and started Proccessing");
        
        List<TestResultDto> results;
        switch (submissionRequestDto.Language)
        {
            case "C++":
                results = await _cppTestingService.TestCppCode(submissionRequestDto);
                break;
            case "Python":
                results = await _pythonTestingService.TestPythonCode(submissionRequestDto);
                break;
            default:
                _logger.LogWarning("Unsupported language: {language}", submissionRequestDto.Language);
                throw new ArgumentException("unsupported language: ", submissionRequestDto.Language);
            break;
        }

        var response = new SubmissionResultResponseDto
        {
            SubmissionId = submissionRequestDto.SubmissionId,
            Results = results
        };

        _rabbitMqService.SendResult(response);
        _logger.LogInformation("results are sent in queue");
    }
}