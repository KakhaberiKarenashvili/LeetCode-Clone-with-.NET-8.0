using MainApp.Domain.Entity;

namespace MainApp.Infrastructure.JWT;

public interface IJwtService
{
    string CreateJwt(User user, IList<string> roles);

}