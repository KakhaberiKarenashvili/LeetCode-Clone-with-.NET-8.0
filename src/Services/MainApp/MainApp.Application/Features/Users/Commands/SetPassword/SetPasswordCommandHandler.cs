using MainApp.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MainApp.Application.Features.Users.Commands.SetPassword;

public class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand, Unit>
{
    private readonly UserManager<User> _userManager;

    public SetPasswordCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            throw new InvalidOperationException("Invalid request");

        var decodedToken = System.Web.HttpUtility.UrlDecode(request.Token);

        var confirmResult = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!confirmResult.Succeeded)
            throw new InvalidOperationException(string.Join("; ", confirmResult.Errors.Select(e => e.Description)));

        var addPasswordResult = await _userManager.AddPasswordAsync(user, request.Password);

        if (!addPasswordResult.Succeeded)
        {
            user.EmailConfirmed = false;
            await _userManager.UpdateAsync(user);
            throw new InvalidOperationException(string.Join("; ", addPasswordResult.Errors.Select(e => e.Description)));
        }

        return Unit.Value;
    }
}
