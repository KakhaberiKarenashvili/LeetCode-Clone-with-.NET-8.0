using MediatR;

namespace MainApp.Application.Features.Cms.Users.AdminResetPassword;

public record AdminResetPasswordCommand(
    string UserId,
    string NewPassword) : IRequest<Unit>;
