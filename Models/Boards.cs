namespace KazeAPI.Models;

public class Boards
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int UserId { get; set; }
    public Users User { get; set; }
    public bool Ativo { get; set; } = true;
    public List<Columns> Columns { get; set; } = new();
}
