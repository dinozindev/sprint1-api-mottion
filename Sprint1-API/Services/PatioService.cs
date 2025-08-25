using Microsoft.EntityFrameworkCore;
using Sprint1_API.Dtos;

namespace Sprint1_API.Services;

public class PatioService
{
    private readonly AppDbContext _db;

    public PatioService(AppDbContext db)
    {
        _db = db;
    }
    
    // retorna todos os patios
    public async Task<IResult> GetAllPatiosAsync()
    {
        var patios = await _db.Patios
            .Include(p => p.Setores)
            .ThenInclude(s => s.Vagas)
            .ToListAsync();
        var patiosDto = patios.Select(PatioReadDto.ToDto).ToList();
        return patiosDto.Any() ? Results.Ok(patiosDto) : Results.NoContent();
    }
    
    // retorna patio por ID
    public async Task<IResult> GetPatioByIdAsync(int id)
    {
        var patio = await _db.Patios
            .Include(p => p.Setores)
            .ThenInclude(s => s.Vagas)
            .FirstOrDefaultAsync(p => p.PatioId == id);
        return patio is null ? Results.NotFound("Nenhum pátio encontrado com ID informado.") : Results.Ok(PatioReadDto.ToDto(patio));
    }
}