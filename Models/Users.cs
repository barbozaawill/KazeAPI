namespace KazeAPI.Models;

public class Users
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<Boards> Boards { get; set; } = new();
    public int? ParentUserId { get; set; }
    public Users? ParentUser { get; set; }
}
