using Microsoft.AspNetCore.Identity;

namespace MainApp.Domain.Entity;

public class User : IdentityUser
{
    public ICollection<Submissions>? Submissions { set; get; }
    
}