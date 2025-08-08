using KazeAPI.DTO;
using KazeAPI.Models;
using KazeAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KazeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public UsersController(UsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            var users = await _usersService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Users>> GetUsers(int id)
        {
            var user = await _usersService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateUsers(Users users)
        {
            var created = await _usersService.CreateAsync(users);
            if (created == null)
                return Conflict("E-mail já cadastrado.");
            return CreatedAtAction(nameof(GetUsers), new { id = created.Id }, created);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO register)
        {
            var result = await _usersService.RegisterAsync(register);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO login)
        {
            var result = await _usersService.LoginAsync(login);
            if (!result.Success) return Unauthorized(result.Message);
            return Ok(result.Message);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateUsers(int id, Users updateUser)
        {
            if (id != updateUser.Id) return BadRequest("ID inválido.");

            var success = await _usersService.UpdateAsync(id, updateUser);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteUsers(int id)
        {
            var success = await _usersService.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
