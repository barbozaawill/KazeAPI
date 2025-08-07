namespace KazeAPI.Models;

public class Cards
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CardNumber { get; set; } // numero do card
    public int Position { get; set; } // posicao que ele vai assumir dentro da coluna
    public int ColumnId {  get; set; }
    public Columns Columns { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // horario de criacao do card
    public DateTime? UpdatedAt { get; set; } // data de atualização do card
    public bool IsCompleted { get; set; } // se foi marcado como concluido ou nao
}
