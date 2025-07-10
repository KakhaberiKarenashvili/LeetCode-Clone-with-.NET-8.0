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
        
        var checkForUnSuccessful = context.Message.Results?.FirstOrDefault(r => r.Success == false) 
                                   ?? context.Message.Results?.FirstOrDefault();

        submission.Status = checkForUnSuccessful?.Status;
        submission.Output = checkForUnSuccessful?.Output; 
        submission.Input = checkForUnSuccessful?.Input;
        submission.ExpectedOutput = checkForUnSuccessful?.ExpectedOutput;

        try
        {
            _appDbContext.Update(submission);

            await _appDbContext.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            throw;
        }
    }
}