using KazeAPI.Models;

namespace KazeAPI.Services;

public interface ITokenService
{
    string GenerateToken(Users usuarios);
}
