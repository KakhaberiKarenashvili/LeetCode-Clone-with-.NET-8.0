using MediatR;

namespace MainApp.Application.Features.Users.Commands.RegisterUser;

public record RegisterUserCommand(
    string UserName,
    string Email,
    string Password) : IRequest<Unit>;
