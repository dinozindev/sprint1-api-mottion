using System.ComponentModel;
using Sprint1_API.Dtos;
using Sprint1_API.Services;

namespace Sprint1_API.Endpoints;

public static class GerenteEndpoints
{
    public static void MapGerenteEndpoints(this IEndpointRouteBuilder app)
    {
        var gerentes = app.MapGroup("/gerentes").WithTags("Gerentes");
        
        gerentes.MapGet("/", async (GerenteService service) => await service.GetAllGerentesAsync())
            .WithSummary("Retorna a lista de gerentes")
            .WithDescription("Retorna a lista de gerentes cadastrados.")
            .Produces<List<GerenteReadDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status500InternalServerError);
        
        gerentes.MapGet("/{id:int}", async ([Description("Identificador único de Gerente")] int id, GerenteService service) => await service.GetGerenteByIdAsync(id))
            .WithSummary("Retorna um gerente pelo ID")
            .WithDescription("Retorna um gerente a partir de um ID. Retorna 200 OK se o gerente for encontrado, ou erro se não for achado.")
            .Produces<GerenteReadDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}