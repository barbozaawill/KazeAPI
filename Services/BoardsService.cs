using KazeAPI.Models;
using KazeAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace KazeAPI.Services;

public class BoardsService
{
    private readonly AppDbContext _context;
    public BoardsService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Boards>> GetAllAsync(bool? somenteAtivos = true)
    {
        IQueryable<Boards> query = _context.Boards;
        if (somenteAtivos.HasValue)
        {
            query = query.Where(b => b.Ativo == somenteAtivos.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<Boards?> GetByIdAsync(int id)
    {
        return await _context.Boards
            .FirstOrDefaultAsync(b => b.Id == id && b.Ativo);
    }

    public async Task<Boards?> CreateAsync(Boards boards)
    {
        bool existe = await _context.Boards.AnyAsync(b => b.Title == boards.Title && b.Ativo);

        if (existe)
            return null;

        boards.Ativo = true;
        boards.CreatedAt = DateTime.UtcNow;

        _context.Boards.Add(boards);
        await _context.SaveChangesAsync();

        return boards;
    }

    public async Task<Boards?> UpdateAsync(Boards boards)
    {
        var existing = await _context.Boards.FindAsync(boards.Id);

        if (existing == null || !existing.Ativo)
            return null;

        existing.Title = boards.Title;
        existing.Description = boards.Description;

        _context.Boards.Update(existing);
        await _context.SaveChangesAsync();

        return existing;
    }

    public async Task<Boards?> DeleteAsync(int id)
    {
        var board = await _context.Boards.FindAsync(id);

        if (board == null || !board.Ativo) 
            return null;

        board.Ativo = false;

        _context.Boards.Update(board);
        await _context.SaveChangesAsync();

        return board;
    }
}
