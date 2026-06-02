using MediatR;

namespace MainApp.Application.Features.Users.Commands.AdminResetPassword;

public record AdminResetPasswordCommand(
    string UserId,
    string NewPassword) : IRequest<Unit>;
