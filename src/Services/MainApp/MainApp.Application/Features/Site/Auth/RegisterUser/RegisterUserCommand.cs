using MediatR;

namespace MainApp.Application.Features.Site.Auth.RegisterUser;

public record RegisterUserCommand(
    string UserName,
    string Email,
    string Password) : IRequest<Unit>;
