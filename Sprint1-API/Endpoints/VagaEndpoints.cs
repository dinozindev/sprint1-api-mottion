using System.ComponentModel;
using Sprint1_API.Dtos;
using Sprint1_API.Services;

namespace Sprint1_API.Endpoints;

public static class VagaEndpoints
{
    public static void MapVagaEndpoints(this IEndpointRouteBuilder app)
    {
        var vagas = app.MapGroup("/vagas").WithTags("Vagas");
        
        vagas.MapGet("/", async (VagaService service) => await service.GetAllVagasAsync())
            .WithSummary("Retorna a lista de vagas")
            .WithDescription("Retorna a lista de vagas cadastradas.")
            .Produces<List<VagaReadDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status500InternalServerError);
        
        vagas.MapGet("/{id:int}", async ([Description("Identificador único de Vaga")] int id, VagaService service) => await service.GetVagaByIdAsync(id))
            .WithSummary("Retorna um patio pelo ID")
            .WithDescription("Retorna um patio a partir de um ID. Retorna 200 OK se o patio for encontrado, ou erro se não for achado.")
            .Produces<PatioReadDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
        }
    }
