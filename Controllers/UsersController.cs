using KazeAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using KazeAPI.DTO;
using Microsoft.EntityFrameworkCore;

namespace KazeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet] 
    public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Users>> GetUsers(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();
       
        return user;
    }

    [HttpPost]
    public async Task<ActionResult<Users>> CreateUsers(Users users)
    {
        
        var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == users.Email);
        if (existing != null)
        {
            return Conflict("E-mail já cadastrado.");
        }

        users.PasswordHash = BCrypt.Net.BCrypt.HashPassword (users.PasswordHash);

        _context.Users.Add(users);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUsers), new { id = users.Id }, users);
    }

    [HttpPost ("login")]
    public async Task<ActionResult> Login([FromBody] LoginDTO login)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u =>u.Email == login.Email);
        if (user == null)
            return Unauthorized("Usuário ou senha inválidos.");

        bool senhaValida = BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash);
        if (!senhaValida)
            return Unauthorized("Usuário ou senha inválidos.");

        return Ok(new { message = "Login efetuado com sucesso!" });
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDTO register)
    {
        if(_context.Users.Any(u => u.Email == register.Email))
        {
            return BadRequest("E-mail já cadastrado.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(register.Password);

        var user = new Users
        {
            Name = register.Name,
            Email = register.Email,
            UserName = register.UserName,
            PasswordHash = passwordHash
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Usuário registrado com sucesso.");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUsers(int id, Users updateUser)
    {
        if (id != updateUser.Id)
            return BadRequest();

        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        user.UserName = updateUser.UserName;
        user.Email = updateUser.Email;
        user.PasswordHash = updateUser.PasswordHash;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUsers(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
