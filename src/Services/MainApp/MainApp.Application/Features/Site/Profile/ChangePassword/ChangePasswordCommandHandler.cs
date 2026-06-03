using MainApp.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MainApp.Application.Features.Site.Profile.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly UserManager<User> _userManager;

    public ChangePasswordCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
            throw new InvalidOperationException("User not found");

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
            throw new InvalidOperationException($"Failed to change password: {string.Join("; ", result.Errors.Select(e => e.Description))}");

        return Unit.Value;
    }
}
