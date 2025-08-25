using System.ComponentModel;
using Sprint1_API.Dtos;
using Sprint1_API.Services;

namespace Sprint1_API.Endpoints;

public static class ClienteEndpoints
{
    public static void MapClienteEndpoints(this IEndpointRouteBuilder app)
    {
        var clientes = app.MapGroup("/clientes").WithTags("Clientes");
        
        clientes.MapGet("/", async (ClienteService service) => await service.GetAllClientesAsync())
            .WithSummary("Retorna a lista de todos os clientes.")
            .WithDescription("Retorna a lista de todos os clientes cadastrados no sistema, incluindo também motos associadas a cada um.")
            .Produces<List<ClienteReadDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status500InternalServerError);
        
        clientes.MapGet("/{id:int}", async ([Description("Identificador único de Cliente")] int id, ClienteService service) => await service.GetClienteByIdAsync(id))
            .WithSummary("Retorna um cliente com sua(s) moto(s) pelo ID")
            .WithDescription("Retorna um cliente e suas motos associadas (caso existam) pelo ID. Retorna 200 OK se o cliente for encontrado, ou erro se não for achado.")
            .Produces<ClienteReadDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}