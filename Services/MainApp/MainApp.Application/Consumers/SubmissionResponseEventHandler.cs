using BuildingBlocks.Messaging.Events;
using MainApp.Application.Dto.Response;
using MainApp.Infrastructure.Data;
using MassTransit;

namespace MainApp.Application.Consumers;

public class SubmissionResponseEventHandler : IConsumer<SubmissionResponseEvent>
{
    
    private readonly AppDbContext _appDbContext;

    public SubmissionResponseEventHandler(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task Consume(ConsumeContext<SubmissionResponseEvent> context)
    {
        var submission =  _appDbContext.Submissions.FirstOrDefault(
            s => s.Id == context.Message.SubmissionId);
        
        if (submission == null)
        {
            throw new InvalidOperationException("Submission not found");
        }
        
        var checkResults = context.Message.Results?.FirstOrDefault(r => r.Success == false) 
                                   ?? context.Message.Results?.FirstOrDefault();

        if (checkResults != null && context.Message.Results != null)
        {
            var total =  context.Message.Results.Count;
            var passed =  context.Message.Results.Count(tr => tr.Success == true);
            
            submission.Status = checkResults.Status;
            submission.Output = checkResults?.Output;
            submission.Input = checkResults?.Input;
            submission.ExpectedOutput = checkResults?.ExpectedOutput; 
            submission.SuccessRate = total == 0 ? 0 : Math.Round((double)(passed / total) * 100, 2);
            
            _appDbContext.Update(submission);

            await _appDbContext.SaveChangesAsync();
        }
        
    }
}