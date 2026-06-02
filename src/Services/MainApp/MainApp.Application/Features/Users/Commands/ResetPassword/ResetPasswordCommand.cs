using MediatR;

namespace MainApp.Application.Features.Users.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string ResetToken,
    string NewPassword) : IRequest<Unit>;
