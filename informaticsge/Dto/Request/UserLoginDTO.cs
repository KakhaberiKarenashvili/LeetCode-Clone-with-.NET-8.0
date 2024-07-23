using System.ComponentModel.DataAnnotations;

namespace informaticsge.Dto;

public class UserLoginDto
{
    [Required]
    public string Email { set; get; }

    [Required]
    public string Password { set; get; }
}