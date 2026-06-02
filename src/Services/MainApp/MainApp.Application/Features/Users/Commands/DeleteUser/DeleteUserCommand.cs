using MediatR;

namespace MainApp.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(string Id) : IRequest<Unit>;
