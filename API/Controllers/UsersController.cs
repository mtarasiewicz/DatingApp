using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    public UsersController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await _dbContext.Users.ToListAsync();
        return Ok(users);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> GetUserById(Guid id)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id.ToString() == id.ToString());
        return Ok(user);
    }
}
