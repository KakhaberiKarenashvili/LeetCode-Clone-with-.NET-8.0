using MediatR;

namespace MainApp.Application.Features.Users.Commands.ChangePassword;

public record ChangePasswordCommand(
    string UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<Unit>;
