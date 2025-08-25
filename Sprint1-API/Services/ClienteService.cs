using Microsoft.EntityFrameworkCore;
using Sprint1_API.Dtos;

namespace Sprint1_API.Services;

public class ClienteService
{
    private readonly AppDbContext _db;

    public ClienteService(AppDbContext db)
    {
        _db = db;
    }
    
    // retorna todos os clientes
    public async Task<IResult> GetAllClientesAsync()
    {
        var clientes = await _db.Clientes.ToListAsync();
        var clientesDto = clientes.Select(ClienteReadDto.ToDto).ToList();
        return clientesDto.Any() ? Results.Ok(clientesDto) : Results.NoContent();
    }

    // busca o cliente pelo ID
    public async Task<IResult> GetClienteByIdAsync(int id)
    {
        var cliente = await _db.Clientes.Include(c => c.Motos).FirstOrDefaultAsync(c => c.ClienteId == id);
        return cliente is null 
            ? Results.NotFound("Cliente não encontrado com ID informado.") 
            : Results.Ok(ClienteReadDto.ToDto(cliente));
    }
    
}