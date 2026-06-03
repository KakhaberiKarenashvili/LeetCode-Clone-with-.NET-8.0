using MediatR;

namespace MainApp.Application.Features.Site.Auth.ResetPassword;

public record ResetPasswordCommand(string Email, string ResetToken, string NewPassword) : IRequest<Unit>;
