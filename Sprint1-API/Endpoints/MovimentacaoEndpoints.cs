using System.ComponentModel;
using Sprint1_API.Dtos;
using Sprint1_API.Services;

namespace Sprint1_API.Endpoints;

public static class MovimentacaoEndpoints
{
    public static void MapMovimentacaoEndpoints(this IEndpointRouteBuilder app)
    {
        var movimentacoes = app.MapGroup("/movimentacoes").WithTags("Movimentações");

        movimentacoes.MapGet("/", async (MovimentacaoService service) => await service.GetAllMovimentacoesAsync())
            .WithSummary("Retorna a lista de movimentações")
            .WithDescription("Retorna a lista de movimentações feitas, com dados da moto, cliente e vaga.")
            .Produces<List<MovimentacaoReadDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status500InternalServerError);
        
        movimentacoes.MapGet("/{id:int}", async ([Description("Identificador único de Movimentação")] int id, MovimentacaoService service) => await service.GetMovimentacaoByIdAsync(id))
            .WithSummary("Retorna uma movimentação pelo ID")
            .WithDescription("Retorna uma movimentação a partir de um ID. Retorna 200 OK se a movimentação for encontrada, ou erro se não for achada.")
            .Produces<MovimentacaoReadDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
        
        movimentacoes.MapGet("/por-moto/{motoId}", async ([Description("Identificador único de Moto")] int motoId, MovimentacaoService service) => await service.GetMovimentacoesByMotoIdAsync(motoId))
            .WithSummary("Retorna movimentações de uma moto específica")
            .WithDescription("Retorna a lista de movimentações associadas a uma moto.")
            .Produces<List<MovimentacaoReadDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
        
        movimentacoes.MapGet("/ocupacao-por-setor/patio/{id}", async ([Description("Identificador único de Movimentação")] int id, MovimentacaoService service) => await service.GetTotalVagasTotalOcupadasAsync(id))
            .WithSummary("Retorna o total de vagas por setor")
            .WithDescription("Retorna o total de vagas e o total de vagas ocupadas por setor a partir do ID de um pátio.")
            .Produces<List<VagasSetorDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
        
        movimentacoes.MapPost("/", async (MovimentacaoPostDto dto, MovimentacaoService service) => await service.CreateMovimentacaoAsync(dto))
            .Accepts<MovimentacaoPostDto>("application/json")
            .WithSummary("Cria uma nova movimentação")
            .WithDescription("Cria uma nova movimentação no sistema, atualizando o status da moto e o status da vaga.")
            .Produces<MovimentacaoReadDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);
        
        movimentacoes.MapPut("/{id:int}/saida", async ([Description("Identificador único de Movimentação")] int id, MovimentacaoService service) => await service.UpdateMovimentacaoAsync(id))
            .WithSummary("Atualiza a data de saída da movimentação.")
            .WithDescription("Altera a data de saída de uma movimentação, finalizando-a. Atualiza a situação da moto para 'Em Trânsito' e desocupa a vaga.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }   
}