using MainApp.Domain.Entity;
using MainApp.Infrastructure.JWT;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MainApp.Application.Features.Users.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;

    public LoginUserCommandHandler(UserManager<User> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new InvalidOperationException("Invalid email or password");

        if (!await _userManager.IsEmailConfirmedAsync(user))
            throw new InvalidOperationException("Please confirm your email before logging in");

        var roles = await _userManager.GetRolesAsync(user);

        return _jwtService.CreateJwt(user, roles);
    }
}
