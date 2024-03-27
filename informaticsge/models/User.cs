using System.ComponentModel.DataAnnotations;
using informaticsge.Models;
using Microsoft.AspNetCore.Identity;

namespace informaticsge.models;

public class User : IdentityUser
{
    
    //IdentityUser has already have its own base model there is no need for me tu implement anything more than solutions which are connected to solution database
    public ICollection<Solution>? Solutions { set; get; }
    
    }