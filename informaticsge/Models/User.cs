using System.ComponentModel.DataAnnotations;
using informaticsge.Models;
using Microsoft.AspNetCore.Identity;

namespace informaticsge.models;

public class User : IdentityUser
{
    public ICollection<Submissions>? Submissions { set; get; }
    
    }