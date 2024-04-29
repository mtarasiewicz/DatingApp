using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;

    public AccountController(AppDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }
    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
    {
        if (await UserExists(request.UserName))
        {
            return BadRequest("Username is already taken.");
        }
        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserName = request.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
            PasswordSalt = hmac.Key
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return new UserResponse { UserName = user.UserName, Token = _tokenService.CreateToken(user) };
    }
    private async Task<bool> UserExists(string userName)
    => await _dbContext.Users.AnyAsync<AppUser>(x => x.UserName == userName.ToLower());

    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> Login(LoginRequest request)
    {
        var user = await _dbContext.Users
            .SingleOrDefaultAsync<AppUser>(x => x.UserName == request.UserName);
        if (user is null) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
        for (var i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }

        return new UserResponse { UserName = user.UserName, Token = _tokenService.CreateToken(user) };

    }

}
