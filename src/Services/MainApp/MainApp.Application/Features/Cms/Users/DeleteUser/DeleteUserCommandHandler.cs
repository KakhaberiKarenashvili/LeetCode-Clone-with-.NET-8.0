using MainApp.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MainApp.Application.Features.Cms.Users.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly UserManager<User> _userManager;

    public DeleteUserCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);

        if (user == null)
            throw new InvalidOperationException("User not found");

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException($"Failed to delete user: {string.Join("; ", result.Errors.Select(e => e.Description))}");

        return Unit.Value;
    }
}
