using MediatR;

namespace MainApp.Application.Features.Users.Commands.ChangeEmail;

public record ChangeEmailCommand(
    string UserId,
    string NewEmail) : IRequest<Unit>;
