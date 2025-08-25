using System.ComponentModel;
using Sprint1_API.Dtos;
using Sprint1_API.Services;

namespace Sprint1_API.Endpoints;

public static class SetorEndpoints
{
   public static void MapSetorEndpoints(this IEndpointRouteBuilder app)
   {
      var setores = app.MapGroup("/setores").WithTags("Setores");

      setores.MapGet("/", async (SetorService service) => await service.GetAllSetoresAsync())
         .WithSummary("Retorna a lista de setores")
         .WithDescription("Retorna a lista de setores cadastrados.")
         .Produces<List<SetorReadDto>>(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status204NoContent)
         .Produces(StatusCodes.Status500InternalServerError);
      
      setores.MapGet("/{id:int}", async ([Description("Identificador único de Setor")] int id, SetorService service) => await service.GetSetorByIdAsync(id))
         .WithSummary("Retorna um setor pelo ID")
         .WithDescription("Retorna um setor a partir de um ID. Retorna 200 OK se o setor for encontrado, ou erro se não for achado.")
         .Produces<SetorReadDto>(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status404NotFound)
         .Produces(StatusCodes.Status500InternalServerError);
   } 
}