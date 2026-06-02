using MediatR;

namespace MainApp.Application.Features.Users.Commands.SetPassword;

public record SetPasswordCommand(
    string Email,
    string Token,
    string Password) : IRequest<Unit>;
