using MediatR;

namespace MainApp.Application.Features.Cms.Users.CreateUser;

public record CreateUserCommand(
    string UserName,
    string Email) : IRequest<Unit>;
