namespace KazeAPI.Models;

public class Columns
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public int Position { get; set; }
    public int BoardId { get; set; }
    public Boards Board { get; set; }
    public List<Cards> Cards { get; set; } = new();
}
