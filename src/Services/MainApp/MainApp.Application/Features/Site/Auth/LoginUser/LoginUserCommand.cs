using MediatR;

namespace MainApp.Application.Features.Site.Auth.LoginUser;

public record LoginUserCommand(
    string Email,
    string Password) : IRequest<string>;
