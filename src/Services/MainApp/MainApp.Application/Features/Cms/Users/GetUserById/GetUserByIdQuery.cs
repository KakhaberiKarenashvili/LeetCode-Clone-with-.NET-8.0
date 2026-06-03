using MainApp.Application.Features.Cms.Users.GetUsers;
using MediatR;

namespace MainApp.Application.Features.Cms.Users.GetUserById;

public record GetUserByIdQuery(string Id) : IRequest<GetUserResponse>;
