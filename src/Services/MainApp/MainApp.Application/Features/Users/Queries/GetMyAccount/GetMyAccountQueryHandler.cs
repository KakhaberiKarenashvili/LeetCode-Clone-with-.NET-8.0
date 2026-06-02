using MainApp.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MainApp.Application.Features.Users.Queries.GetMyAccount;

public class GetMyAccountQueryHandler : IRequestHandler<GetMyAccountQuery, MyAccountResponse>
{
    private readonly UserManager<User> _userManager;

    public GetMyAccountQueryHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<MyAccountResponse> Handle(GetMyAccountQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        return new MyAccountResponse(user?.UserName, user?.Email);
    }
}
