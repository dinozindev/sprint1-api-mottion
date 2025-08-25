using Microsoft.EntityFrameworkCore;
using Sprint1_API.Dtos;

namespace Sprint1_API.Services;

public class FuncionarioService
{
    private readonly AppDbContext _db;

    public FuncionarioService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> GetAllFuncionariosAsync()
    {
        var funcionarios = await _db.Funcionarios
            .Include(f => f.Cargo)
            .Include(f => f.Patio)
            .ToListAsync();
        var funcionariosDto = funcionarios.Select(FuncionarioReadDto.ToDto).ToList();
        return funcionariosDto.Any() ? Results.Ok(funcionariosDto) : Results.NoContent();
        
    }

    public async Task<IResult> GetFuncionarioByIdAsync(int id)
    {
        var funcionario = await _db.Funcionarios
            .Include(f => f.Cargo)
            .Include(f => f.Patio)
            .FirstOrDefaultAsync(f => f.FuncionarioId == id);
        return funcionario is null 
            ? Results.NotFound("Nenhum funcionário encontrado com ID informado.")
            : Results.Ok(FuncionarioReadDto.ToDto(funcionario));
    }
} 