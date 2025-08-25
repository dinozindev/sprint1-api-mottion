using Microsoft.EntityFrameworkCore;
using Sprint1_API.Dtos;

namespace Sprint1_API.Services;

public class GerenteService
{
    private readonly AppDbContext _db;

    public GerenteService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> GetAllGerentesAsync()
    {
        var gerentes = await _db.Gerentes
            .Include(g => g.Patio)
            .ToListAsync();

        var gerentesDto = gerentes.Select(GerenteReadDto.ToDto).ToList();
        
        return gerentesDto.Any() ? Results.Ok(gerentesDto) : Results.NoContent();
    }

    public async Task<IResult> GetGerenteByIdAsync(int id)
    {
        var gerente = await _db.Gerentes
            .Include(g => g.Patio)
            .FirstOrDefaultAsync(g => g.GerenteId == id);
        
        return gerente is null ? Results.NotFound("Nenhum Gerente encontrado com ID informado.") : Results.Ok(GerenteReadDto.ToDto(gerente));
    }
}