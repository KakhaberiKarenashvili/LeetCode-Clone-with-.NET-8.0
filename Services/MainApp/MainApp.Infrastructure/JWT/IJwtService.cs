using MainApp.Domain.Models;

namespace MainApp.Infrastructure.JWT;

public interface IJwtService
{
    string CreateJwt(User user, IList<string> roles);

}