using MediatR;

namespace MainApp.Application.Features.Site.Auth.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Unit>;
