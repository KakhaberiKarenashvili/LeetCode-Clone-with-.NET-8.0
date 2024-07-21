using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using informaticsge.models;
using Microsoft.IdentityModel.Tokens;
using static System.Reflection.MethodBase;

namespace informaticsge.JWT;

public class JWTService
{
    private readonly IConfiguration _config;

    public JWTService(IConfiguration config)
    {
        _config = config;
    }
    public string CreateJwt(User user)
    {

        var userClaims = new List<Claim>  
        {
            new Claim("Id", user.Id),
            new Claim("Email", user.Email),
            new Claim("UserName", user.UserName)
        };
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            userClaims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);
        
        var token = new JwtSecurityTokenHandler().WriteToken(sectoken);
        
        return token;
    }
}