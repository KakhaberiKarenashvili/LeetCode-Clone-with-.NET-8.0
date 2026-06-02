using MainApp.Application.Features.Users.Queries.GetUsers;
using MediatR;

namespace MainApp.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(string Id) : IRequest<GetUserResponse>;
