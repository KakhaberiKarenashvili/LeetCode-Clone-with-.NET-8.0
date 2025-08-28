using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MainApp.Domain.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MainApp.Infrastructure.JWT;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }
    public string  CreateJwt(User user,IList<string> roles)
    {

        var userClaims = new List<Claim>  
        {
            new Claim("Id", user.Id),
            new Claim("UserName", user.UserName),
        };

        foreach (var role in roles)
        {
            userClaims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            userClaims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials);
        
        var tokenToken = new JwtSecurityTokenHandler().WriteToken(sectoken);

        return tokenToken;
    }
}