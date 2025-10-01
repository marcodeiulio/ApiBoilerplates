using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<ActionResult<TokenResponseDto>> Login(UserDto req)
    {
        var user = await _context.Users
        .Include(u => u.Roles)
        .FirstOrDefaultAsync(u => u.Username == req.Username);

        if (user is null)
        {
            return BadRequest("Invalid credentials.");
        }

        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, req.Password)
            == PasswordVerificationResult.Failed)
        {
            return BadRequest("Invalid credentials.");
        }

        var tokenDto = await CreateTokenResponseDto(user);

        return Ok(tokenDto);
    }

    [Authorize]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshTokens(TokenRequestDto request)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user is null
        || user.RefreshToken != request.RefreshToken
        || user.RefreshTokenExpiryDate < DateTime.UtcNow)
        {
            return Unauthorized("Invalid token.");
        }

        var result = await CreateTokenResponseDto(user);

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("admin-dashboard")]
    public ActionResult<string> GetAdminData()
    {
        return "This is super secret admin data.";
    }

    private async Task<TokenResponseDto> CreateTokenResponseDto(User user)
    {
        return new TokenResponseDto
        {
            AccessToken = CreateToken(user),
            RefreshToken = await CreateAndStoreRefreshToken(user)
        };
    }

    [HttpGet("get-roles")]
    public async Task<ActionResult<List<RolesWithUsersDto>>> GetRoles()
    {
        var roles = await _context.Roles
            .Include(r => r.Users)
            .ToListAsync();

        var rolesDtos = roles.Select(role => new RolesWithUsersDto
        {
            Id = role.Id,
            Name = role.Name,
            Users = role.Users.Select(user => new UserDto
            {
                Username = user.Username
            }).ToList()
        }).ToList();

        return rolesDtos;
    }

    [HttpGet("get-user-by-id")]
    public async Task<ActionResult<object>> GetUser(Guid userGuid)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userGuid);

        if (user is null)
            return NotFound("User not found.");

        var result = new
        {
            Id = user.Id,
            Username = user.Username,
            Roles = user.Roles.Select(r => new RolesDto
            {
                Id = r.Id,
                Name = r.Name
            }).ToList()
        };

        return Ok(result);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        foreach (Roles role in user.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role.Name));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf["Auth:Key"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _conf["Auth:Issuer"],
            audience: _conf["Auth:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    private async Task<string> CreateAndStoreRefreshToken(User user)
    {
        user.RefreshToken = CreateRefreshToken();
        user.RefreshTokenExpiryDate = DateTime.UtcNow.AddMinutes(5);
        await _context.SaveChangesAsync();
        return user.RefreshToken;
    }

    private static string CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}