using MainApp.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MainApp.Application.Features.Cms.Users.AdminResetPassword;

public class AdminResetPasswordCommandHandler : IRequestHandler<AdminResetPasswordCommand, Unit>
{
    private readonly UserManager<User> _userManager;

    public AdminResetPasswordCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(AdminResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
            throw new InvalidOperationException("User not found");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

        if (!result.Succeeded)
            throw new InvalidOperationException($"Failed to reset password: {string.Join("; ", result.Errors.Select(e => e.Description))}");

        return Unit.Value;
    }
}
