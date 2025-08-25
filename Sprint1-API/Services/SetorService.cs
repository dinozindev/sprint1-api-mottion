using Microsoft.EntityFrameworkCore;
using Sprint1_API.Dtos;

namespace Sprint1_API.Services;

public class SetorService
{
    private readonly AppDbContext _db;

    public SetorService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> GetAllSetoresAsync()
    {
        var setores = await _db.Setores
            .Include(s => s.Patio)
            .Include(s => s.Vagas)
            .ToListAsync();

        var setoresDto = setores
            .Select(SetorReadDto.ToDto)
            .ToList();
        
        return setoresDto.Any() ? Results.Ok(setoresDto) : Results.NoContent();
    }

    public async Task<IResult> GetSetorByIdAsync(int id)
    {
        var setor = await _db.Setores
            .Include(s => s.Patio)
            .Include(s => s.Vagas)
            .FirstOrDefaultAsync(s => s.SetorId == id);
        
        return setor is null ? Results.NotFound("Nenhum setor encontrado com ID informado.") : Results.Ok(SetorReadDto.ToDto(setor));
    }
}