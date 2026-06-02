using MediatR;

namespace MainApp.Application.Features.Users.Commands.ConfirmEmail;

public record ConfirmEmailCommand(
    string Email,
    string Token) : IRequest<Unit>;
