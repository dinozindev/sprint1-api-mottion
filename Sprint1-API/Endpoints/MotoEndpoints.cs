using System.ComponentModel;
using Sprint1_API.Dtos;
using Sprint1_API.Model;
using Sprint1_API.Services;

namespace Sprint1_API.Endpoints;

public static class MotoEndpoints
{
    public static void MapMotoEndpoints(this IEndpointRouteBuilder app)
    {
        var motos = app.MapGroup("/motos").WithTags("Motos");
        
        motos.MapGet("/", async (MotoService service) => await service.GetAllMotosAsync())
            .WithSummary("Retorna uma lista contendo todas as motos.")
            .WithDescription("Retorna a lista de todas as motos cadastradas no sistema. O id do cliente pode ser nulo ou não.")
            .Produces<List<MotoReadDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status500InternalServerError);
        
        motos.MapGet("/{id:int}", async ([Description("Identificador único de Moto")] int id, MotoService service) => await service.GetMotoByIdAsync(id))
            .WithSummary("Retorna uma moto pelo ID")
            .WithDescription("Retorna uma moto pelo ID. Retorna 200 OK se a moto for encontrada, ou erro se não for achada.")
            .Produces<MotoReadDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
        
        motos.MapGet("/por-chassi/{numeroChassi}", async ([Description("Número de Chassi único da Moto")] string numeroChassi, MotoService service) => await service.GetMotoByChassiAsync(numeroChassi))
            .WithSummary("Retorna uma moto pelo Número de Chassi")
            .WithDescription("Retorna uma moto pelo número de Chassi. Retorna 200 OK se a moto for encontrada, ou erro se não for achada.")
            .Produces<MotoReadDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
        
        motos.MapGet("/{id:int}/ultima-posicao", async ([Description("Identificador único de Moto")] int id, MotoService service) => await service.GetMotoUltimaPosicaoAsync(id))
            .WithSummary("Retorna a última posição da moto")
            .WithDescription("Retorna a última vaga e setor em que a moto esteve, com base na movimentação mais recente.")
            .Produces<UltimaPosicaoDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
        
        motos.MapPost("/", async (MotoPostDto dto, MotoService service) => await service.CreateMotoAsync(dto))
            .Accepts<MotoPostDto>("application/json")
            .WithSummary("Cria uma moto")
            .WithDescription("Cria uma moto no sistema.")
            .Produces<Moto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);
        
        motos.MapPut("/{id:int}", async ([Description("Identificador único de Moto")] int id, MotoPostDto dto, MotoService service) => await service.UpdateMotoAsync(id, dto))
            .Accepts<MotoPostDto>("application/json")
            .WithSummary("Atualiza uma moto")
            .WithDescription("Atualiza os dados de uma moto existente.")
            .Produces<Moto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);
        
        motos.MapDelete("/{id:int}", async ([Description("Identificador único de Moto")] int id, MotoService service) => await service.DeleteMotoAsync(id))
            .WithSummary("Deleta uma moto pelo ID")
            .WithDescription("Retorna uma moto pelo ID informado. Retorna 204 No Content caso encontrado, ou erro se não achado.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
        
        motos.MapPut("/{id:int}/remover-cliente", async ([Description("Identificador único de Moto")] int id, MotoService service) => await service.DeleteAssociacaoClienteMotoAsync(id))
            .WithSummary("Remove a associação do cliente a moto.")
            .WithDescription("Remove a associação do cliente de uma moto através do ID da moto.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        
        motos.MapPut("/{id:int}/alterar-cliente/{clienteId}", async ([Description("Identificador único de Moto")] int id, [Description("Identificador único de Cliente")] int clienteId, MotoService service) => await service.UpdateAssociacaoClienteMotoAsync(id, clienteId))
            .WithSummary("Altera a associação do cliente a moto.")
            .WithDescription("Altera a associação do cliente de uma moto através do ID da moto e do cliente.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }    
}