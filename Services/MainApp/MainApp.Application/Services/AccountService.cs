using MainApp.Application.Dto.Request;
using MainApp.Domain.Entity;
using MainApp.Infrastructure.JWT;
using Microsoft.AspNetCore.Identity;

namespace MainApp.Application.Services;

public class AccountService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;

    public AccountService(UserManager<User> userManager, IJwtService jwtService)
    {
        _jwtService = jwtService;
        _userManager = userManager;
    }
    
    public async Task Register(RegistrationDto newUser)
    {
       if (await CheckEmailExists(newUser.Email))
       {
           throw new Exception("Email already exists");
       }
     

       if (await CheckUsernameExists(newUser.UserName))  
       {
           throw new Exception("username already exists");
       }
       
       var userToAdd = new User()
       {
           Email = newUser.Email,
           UserName = newUser.UserName,
       };
       
       //errors are quite tricky hard to debug in case of errors use debugger 
       var createProcess = await _userManager.CreateAsync(userToAdd, newUser.Password);
        
       if (!createProcess.Succeeded)
       {
           var errorMessage = string.Join("\n", createProcess.Errors.Select(e => e.Description));
           
           throw new AggregateException(errorMessage); 
       }

       var roleAssignmentResult = await _userManager.AddToRoleAsync(userToAdd, "User");
      
       if (!roleAssignmentResult.Succeeded)
       {
           var roleErrorMessage = string.Join("\n", roleAssignmentResult.Errors.Select(e => e.Description));
           
           throw new InvalidOperationException("Failed To Assign Role.");
       }
    }

    
    public async Task<string> Login(UserLoginDto userLogin)
    {
        var user = await _userManager.FindByEmailAsync(userLogin.Email);

        if (user == null)
        {
            throw new InvalidOperationException("Invalid Email or Password");
        }

        var checkPassword = await _userManager.CheckPasswordAsync(user, userLogin.Password);

        if (checkPassword == false)
        {
            throw new InvalidOperationException("Invalid Email or Password");
        }

        var userRole = await _userManager.GetRolesAsync(user);
        
        try
        {
            var jwt = _jwtService.CreateJwt(user,userRole);
            
            return jwt;
        }
        catch(Exception ex)
        {
            throw new Exception("Failed To Generate JWT Token");
        }
    }
    
    
    private async Task<bool> CheckEmailExists(string email)
    {
      var result =  await _userManager.FindByEmailAsync(email);
     
      if (result == null)
      {
          return false;
      }
      return true;
    }
    
    private async Task<bool> CheckUsernameExists(string username)
    {
        var result = await _userManager.FindByNameAsync(username);
        
        if (result == null)
        {
            return false;
        }
        return true;
    }
    
}