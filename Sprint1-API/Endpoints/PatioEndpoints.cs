using System.ComponentModel;
using Sprint1_API.Dtos;
using Sprint1_API.Services;

namespace Sprint1_API.Endpoints;

public static class PatioEndpoints
{
    public static void MapPatioEndpoints(this IEndpointRouteBuilder app)
    {
        var patios = app.MapGroup("/patios").WithTags("Patios");
        
        patios.MapGet("/", async (PatioService service) => await service.GetAllPatiosAsync())
            .WithSummary("Retorna a lista de pátios com setores e vagas")
            .WithDescription("Retorna todos os pátios cadastrados, com seus respectivos setores e vagas.")
            .Produces<List<PatioReadDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status500InternalServerError);
        
        patios.MapGet("/{id:int}", async ([Description("Identificador único de Pátio")] int id, PatioService service) => await service.GetPatioByIdAsync(id))
            .WithSummary("Retorna um patio pelo ID")
            .WithDescription("Retorna um patio a partir de um ID. Retorna 200 OK se o patio for encontrado, ou erro se não for achado.")
            .Produces<PatioReadDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}