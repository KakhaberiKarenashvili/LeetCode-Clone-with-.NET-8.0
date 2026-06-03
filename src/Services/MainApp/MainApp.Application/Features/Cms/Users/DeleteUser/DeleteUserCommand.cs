using MediatR;

namespace MainApp.Application.Features.Cms.Users.DeleteUser;

public record DeleteUserCommand(string Id) : IRequest<Unit>;
