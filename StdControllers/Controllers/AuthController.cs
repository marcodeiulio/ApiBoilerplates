using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StdControllers.Data;
using StdControllers.Models;

namespace StdControllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    IConfiguration _conf;
    protected readonly ApplicationDbContext _context;

    public AuthController(IConfiguration configuration, ApplicationDbContext context)
    {
        _conf = configuration;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserDto req)
    {
        if (await _context.Users.AnyAsync<User>(u => u.Username == req.Username))
        {
            return BadRequest("Username already exists");
        }

        var user = new User();

        var hashedPassword = new PasswordHasher<User>()
        .HashPassword(user, req.Password);

        user.Username = req.Username;
        user.PasswordHash = hashedPassword;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UserDto req)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == req.Username);

        if (user is null)
        {
            return BadRequest("Invalid credentials.");
        }

        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, req.Password)
            == PasswordVerificationResult.Failed)
        {
            return BadRequest("Invalid credentials.");
        }

        var token = CreateToken(user);
        return Ok(token);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf["Auth:Key"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _conf["Auth:Issuer"],
            audience: _conf["Auth:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}