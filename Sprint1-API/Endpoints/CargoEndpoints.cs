using System.ComponentModel;
using Sprint1_API.Dtos;
using Sprint1_API.Services;

namespace Sprint1_API.Endpoints;

public static class CargoEndpoints
{
    public static void MapCargoEndpoints(this IEndpointRouteBuilder app)
    {
        var cargos = app.MapGroup("/cargos").WithTags("Cargos");
        
        cargos.MapGet("/", async (CargoService service) => await service.GetAllCargosAsync())
            .WithSummary("Retorna a lista de cargos.")
            .WithDescription("Retorna a lista de cargos cadastrados.")
            .Produces<List<CargoReadDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status500InternalServerError);
        
        cargos.MapGet("/{id:int}", async ([Description] int id, CargoService service) => await service.GetCargoByIdAsync(id))
            .WithSummary("Retorna um cargo pelo ID")
            .WithDescription("Retorna um cargo a partir de um ID. Retorna 200 OK se o cargo for encontrado, ou erro se não for achado.")
            .Produces<CargoReadDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}