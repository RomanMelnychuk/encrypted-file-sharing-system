using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FileGuard.API.Extensions;
using FileGuard.Identity.Models;
using FileGuard.Shared.Extensions;
using FileGuard.Storage.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FileGuard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticateController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IFolderService folder,
    IConfiguration configuration)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new User { Id = Guid.NewGuid().ToString(), UserName = model.Username, Email = model.Email };

        var result = await userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            folder.Create(user.Id.ToUserRootFolderName());
            return Ok(new { message = "User registered successfully" });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

        if (!result.Succeeded) return Unauthorized();

        var user = await userManager.FindByNameAsync(model.Username);

        if (user == null)
        {
            return BadRequest("User not exist with this username!");
        }

        var token = GenerateJwtToken(user);

        HttpContext.SetAuthCookies(token);

        return Ok();
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim("userId", user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public class RegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
