using KazeAPI.Models;
using KazeAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using KazeAPI.DTO;
using Microsoft.AspNetCore.Authorization;

namespace KazeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardsController : ControllerBase  
{
    private readonly BoardsService _boardService;

    public BoardsController(BoardsService boardService)
    {
        _boardService = boardService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Boards>>> GetBoards([FromQuery] bool? somenteAtivos = true)
    {
        var boards = await _boardService.GetAllAsync(somenteAtivos);
        return Ok(boards);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Boards>> GetBoard(int id)
    {
        var board = await _boardService.GetByIdAsync(id);
        if (board == null) return NotFound();
        return Ok(board);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Boards>> CreateBoard([FromBody]RegisterBoardDTO register)
    {
        var usersIdClaim = User.FindFirst("id");

        if (usersIdClaim == null)
            return Unauthorized("Usuário não autenticado.");

        int userId = int.Parse(usersIdClaim.Value);

        var board = new Boards
        {
            Title = register.Title,
            Description = register.Description,
            UserId = userId,
            Ativo = true,
            CreatedAt = DateTime.UtcNow,
        };

        var createdBoard = await _boardService.CreateAsync(board);
        if (createdBoard == null)
            return Conflict("Título já cadastrado!");
                
        return CreatedAtAction(nameof(GetBoard), new { id = createdBoard.Id }, createdBoard);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<Boards>> UpdateBoard(int id, [FromBody] UpdateBoardDTO dto)
    {
        var existing = await _boardService.GetByIdAsync(id);
        if (existing == null) return NotFound();

        var userId = int.Parse(User.FindFirst("id").Value);
        if (existing.UserId != userId) return Unauthorized();

        existing.Title = dto.Title;
        existing.Description = dto.Description;

        var updateBoard = await _boardService.UpdateAsync(existing);
        return Ok(updateBoard);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<Boards>> DeleteBoard(int id)
    {
        var board = await _boardService.GetByIdAsync(id);
        if (board == null)
            return NotFound();

        var userId = int.Parse(User.FindFirst("id").Value);
        if (board.UserId != userId)
            return Unauthorized();

        var deletedBoard = await _boardService.DeleteAsync(id);
        return Ok(deletedBoard);
    }
}
