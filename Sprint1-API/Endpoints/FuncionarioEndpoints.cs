using System.ComponentModel;
using Sprint1_API.Dtos;
using Sprint1_API.Services;

namespace Sprint1_API.Endpoints;

public static class FuncionarioEndpoints
{
    public static void MapFuncionarioEndpoints(this IEndpointRouteBuilder app)
    {
        var funcionarios = app.MapGroup("funcionarios").WithTags("Funcionários");
        
        funcionarios.MapGet("/", async (FuncionarioService service) => await service.GetAllFuncionariosAsync())
            .WithSummary("Retorna a lista de funcionários")
            .WithDescription("Retorna a lista de funcionários cadastrados.")
            .Produces<List<FuncionarioReadDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status500InternalServerError);
        
        funcionarios.MapGet("/{id:int}", async ([Description("Identificador único de Funcionário")] int id, FuncionarioService service) => await service.GetFuncionarioByIdAsync(id))
            .WithSummary("Retorna um funcionário pelo ID")
            .WithDescription("Retorna um funcionário a partir de um ID. Retorna 200 OK se o funcionário for encontrado, ou erro se não for achado.")
            .Produces<FuncionarioReadDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
        }
}
