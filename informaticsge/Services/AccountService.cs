using informaticsge.Dto.Request;
using informaticsge.JWT;
using informaticsge.Models;
using Microsoft.AspNetCore.Identity;


namespace informaticsge.Services;

public class AccountService
{
    private readonly UserManager<User> _userManager;
    private readonly JWTService _jwtService;
    private readonly ILogger<AccountService> _logger;

    public AccountService(UserManager<User> userManager, JWTService jwtService, ILogger<AccountService> logger)
    {
        _jwtService = jwtService;
        _logger = logger;
        _userManager = userManager;
    }
    
    public async Task Register(RegistrationDto newUser)
    {
        _logger.LogInformation("Started Registration Process for Username: {username}", newUser.UserName);
        
       if (await CheckEmailExists(newUser.Email))
       {
           _logger.LogError("Email Already Exists {email}", newUser.Email);

           throw new Exception("Email already exists");
       }
     

       if (await CheckUsernameExists(newUser.UserName))  
       {
           _logger.LogError("Username Already Exists {username}",newUser.UserName);

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
          
           _logger.LogError(errorMessage);
           
           throw new AggregateException(errorMessage); 
       }
       
    }

    
    public async Task<string> Login(UserLoginDto userLogin)
    {
        _logger.LogInformation("Started Login Process for Email: {email}", userLogin.Email);
        
        var user = await _userManager.FindByEmailAsync(userLogin.Email);

        if (user == null)
        {
            _logger.LogError("User Not Found For Email: {email}", userLogin.Email);

            throw new InvalidOperationException("Invalid Email or Password");
        }

        var checkPassword = await _userManager.CheckPasswordAsync(user, userLogin.Password);

        if (checkPassword == false)
        {
            _logger.LogError("wrong password");
            
            throw new InvalidOperationException("Invalid Email or Password");
        }
        
        try
        {
            _logger.LogInformation("Started generating JWT for UserName: {username}", user.UserName);
            
            var jwt = _jwtService.CreateJwt(user);
            
            return jwt;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to generate JWT for Username: {username}", user.UserName);
            throw; 
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