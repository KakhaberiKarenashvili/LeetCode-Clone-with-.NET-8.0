using MediatR;

namespace MainApp.Application.Features.Site.Auth.SetPassword;

public record SetPasswordCommand(string Email, string Token, string Password) : IRequest<Unit>;
