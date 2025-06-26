using Microsoft.AspNetCore.Identity;

namespace MainApp.Domain.Models;

public class User : IdentityUser
{
    public ICollection<Submissions>? Submissions { set; get; }
    
}