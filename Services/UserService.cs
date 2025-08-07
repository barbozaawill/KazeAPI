using KazeAPI.DTO;
using KazeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KazeAPI.Services
{
    public class UsersService
    {
        private readonly AppDbContext _context;

        public UsersService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<Users?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDTO register)
        {
            if (_context.Users.Any(u => u.Email == register.Email))
                return (false, "E-mail já cadastrado.");

            var user = new Users
            {
                Name = register.Name,
                Email = register.Email,
                UserName = register.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(register.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return (true, "Usuário registrado com sucesso.");
        }

        public async Task<(bool Success, string Message)> LoginAsync(LoginDTO login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
                return (false, "Usuário ou senha inválidos.");

            return (true, "Login efetuado com sucesso!");
        }

        public async Task<Users?> CreateAsync(Users user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
                return null;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateAsync(int id, Users updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.UserName = updatedUser.UserName;
            user.Email = updatedUser.Email;
            user.PasswordHash = updatedUser.PasswordHash;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
