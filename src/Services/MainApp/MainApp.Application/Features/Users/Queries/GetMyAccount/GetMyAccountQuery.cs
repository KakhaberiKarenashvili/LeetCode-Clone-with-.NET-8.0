using MediatR;

namespace MainApp.Application.Features.Users.Queries.GetMyAccount;

public record GetMyAccountQuery(string UserId) : IRequest<MyAccountResponse>;
