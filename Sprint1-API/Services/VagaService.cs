using Microsoft.EntityFrameworkCore;
using Sprint1_API.Dtos;

namespace Sprint1_API.Services;

public class VagaService
{
    private readonly AppDbContext _db;

    public VagaService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> GetAllVagasAsync()
    {
        var vagas = await _db.Vagas
            .Include(v => v.Setor)
            .ToListAsync();
        
        var vagasDto = vagas
            .Select(VagaReadDto.ToDto)
            .ToList();
        
        return vagasDto.Any() ? Results.Ok(vagasDto) : Results.NoContent();
    }

    public async Task<IResult> GetVagaByIdAsync(int id)
    {
        var vaga = await _db.Vagas
            .Include(v => v.Setor)
            .FirstOrDefaultAsync(v => v.VagaId == id);
        
        return vaga is null 
            ? Results.NotFound("Nenhuma vaga encontrada com ID informado.") 
            : Results.Ok(VagaReadDto.ToDto(vaga));
    }
}