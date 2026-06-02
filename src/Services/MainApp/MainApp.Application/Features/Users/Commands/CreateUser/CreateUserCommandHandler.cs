using MainApp.Application.Common.Options;
using MainApp.Application.Common.Services.Email;
using MainApp.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MainApp.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Unit>
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly EmailOptions _emailOptions;

    public CreateUserCommandHandler(
        UserManager<User> userManager,
        IEmailService emailService,
        IOptions<EmailOptions> emailOptions)
    {
        _userManager = userManager;
        _emailService = emailService;
        _emailOptions = emailOptions.Value;
    }

    public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
            throw new InvalidOperationException("Email already exists");

        if (await _userManager.FindByNameAsync(request.UserName) != null)
            throw new InvalidOperationException("Username already exists");

        var user = new User
        {
            Email = request.Email,
            UserName = request.UserName
        };

        var createResult = await _userManager.CreateAsync(user);

        if (!createResult.Succeeded)
            throw new InvalidOperationException(string.Join("\n", createResult.Errors.Select(e => e.Description)));

        var roleResult = await _userManager.AddToRoleAsync(user, "Admin");

        if (!roleResult.Succeeded)
            throw new InvalidOperationException("Failed to assign role.");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var baseRoute = $"{_emailOptions.FrontendBaseUrl}/{_emailOptions.SetPasswordRoute}";

        var body = await EmailTemplateProvider.GetEmailBodyAsync(
            request.Email,
            EmailTemplateProvider.SetPasswordTemplate,
            token,
            baseRoute);

        await _emailService.SendEmailAsync(request.Email, EmailSubjects.SetPassword, body, cancellationToken);

        return Unit.Value;
    }
}
