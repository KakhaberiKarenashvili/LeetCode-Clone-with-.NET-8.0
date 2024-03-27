
using informaticsge.Dto;
using informaticsge.entity;
using informaticsge.JWT;
using informaticsge.models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace informaticsge.Services;

public class AccountService
{
    private AppDBcontext _appDBcontext;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JWTService _jwtService;

    public AccountService(AppDBcontext appDBcontext, UserManager<User> userManager, SignInManager<User> signInManager, JWTService jwtService)
    {
        _appDBcontext = appDBcontext;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _userManager = userManager;
    }
    
    public async Task<string> Register(RegistrationDTO newuser)
    {
        
       if (await CheckEmailExists(newuser.Email))
       {
           return "email already here";
       }
     
       //createasync method does it for u but i added it for clarity
       if (await CheckUsernmaeExists(newuser.UserName))  
       {
           return "username already here";
       }
       
       var userToAdd = new User()
       {
           Email = newuser.Email,
           UserName = newuser.UserName,
       };
       
       //errors are quite tricky hard to debug in case of errors use debugger 
       var createprocess = await _userManager.CreateAsync(userToAdd, newuser.Password);
        
       if (!createprocess.Succeeded)
       {
           //made for logging createasync errors not best option but does its job well
           var errors = string.Join("\n ", createprocess.Errors.Select(e=>e.Description));
           return errors;
       }
       return "user added successfully";
    }

    
    public async Task<string> Login(UserLoginDTO userLogin)
    {
        var user = await _userManager.FindByEmailAsync(userLogin.Email);
        
        if (user == null)
        {
            return "Invalid Email or Password";
        }

        var checkpass = await _userManager.CheckPasswordAsync(user, userLogin.Password);

        if (checkpass == false)
        {
            return "Invalid Email or Password";
        }
        
        var jwt = _jwtService.CreateJwt(user);

        return jwt;
    }

    //santas little helpers
    private async Task<bool> CheckEmailExists(string email)
    {
      var result =  await _userManager.FindByEmailAsync(email);
     
      if (result == null)
      {
          return false;
      }
      return true;
    }
    
//i could do it more fancy way but it works sooo... rule 1
    private async Task<bool> CheckUsernmaeExists(string username)
    {
        var result = await _userManager.FindByNameAsync(username);
        if (result == null)
        {
            return false;
        }
        return true;
    }
    
}