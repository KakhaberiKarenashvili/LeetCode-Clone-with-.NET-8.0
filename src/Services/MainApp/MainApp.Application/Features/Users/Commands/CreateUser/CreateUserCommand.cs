using MediatR;

namespace MainApp.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(
    string UserName,
    string Email) : IRequest<Unit>;
