using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sprint1_API.Dtos;
using Sprint1_API.Model;

namespace Sprint1_API.Services;

public class MovimentacaoService
{
    private readonly AppDbContext _db;
    private readonly IHubContext<SetorHub> _hubContext;

    public MovimentacaoService(AppDbContext db, IHubContext<SetorHub> hubContext)
    {
        _db = db;
        _hubContext = hubContext;
    }

    public async Task<IResult> GetAllMovimentacoesAsync()
    {
        var movimentacoes = await _db.Movimentacoes
            .Include(m => m.Moto)
            .ThenInclude(mo => mo.Cliente)
            .Include(m => m.Vaga)
            .ThenInclude(v => v.Setor)
            .ThenInclude(s => s.Patio)
            .ToListAsync();

        var movimentacoesDto = movimentacoes.Select(MovimentacaoReadDto.ToDto).ToList();
        
        return movimentacoesDto.Any() ? Results.Ok(movimentacoesDto) : Results.NoContent();
    }

    public async Task<IResult> GetMovimentacaoByIdAsync(int id)
    {
        var movimentacao = await _db.Movimentacoes
            .Include(m => m.Moto)
            .ThenInclude(m => m.Cliente)
            .Include(m => m.Vaga)
            .ThenInclude(v => v.Setor)
            .ThenInclude(s => s.Patio)
            .FirstOrDefaultAsync(s => s.MovimentacaoId == id);
        
        return movimentacao is null ? Results.NotFound("Nenhuma Movimentação encontrada com ID informado.") : Results.Ok(MovimentacaoReadDto.ToDto(movimentacao));
    }

    public async Task<IResult> GetMovimentacoesByMotoIdAsync(int motoId)
    {
        var moto = await _db.Movimentacoes
            .Where(m => m.MotoId == motoId)
            .FirstOrDefaultAsync();
        
        if (moto is null) return Results.NotFound("Nenhuma movimentação encontrada para a Moto informada.");
        
        var movimentacoes = await _db.Movimentacoes
            .Where(m => m.MotoId == motoId)
                .Include(m => m.Moto)
                .ThenInclude(mo => mo.Cliente)
                .Include(m => m.Vaga)
                .ThenInclude(v => v.Setor)
                .ThenInclude(s => s.Patio)
                .ToListAsync();
        
        var movimentacoesDto = movimentacoes.Select(MovimentacaoReadDto.ToDto).ToList();

        return Results.Ok(movimentacoesDto);
    }

    public async Task<IResult> GetTotalVagasTotalOcupadasAsync(int id)
    {
        var resultado = await _db.Setores
            .Where(s => s.PatioId == id)
            .Select(s => new
            {
                Setor = s.TipoSetor,
                TotalVagas = _db.Vagas.Count(v => v.SetorId == s.SetorId),
                MotosPresentes = _db.Movimentacoes.Count(m =>
                    m.DtSaida == null &&
                    _db.Vagas
                        .Where(v => v.SetorId == s.SetorId)
                        .Select(v => v.VagaId)
                        .Contains(m.VagaId)
                )
            })
            .ToListAsync();

        return Results.Ok(resultado);
    }

    public async Task<IResult> CreateMovimentacaoAsync(MovimentacaoPostDto dto)
    {
        var movimentacao = new Movimentacao
    {
        DescricaoMovimentacao = dto.DescricaoMovimentacao,
        MotoId = dto.MotoId,
        VagaId = dto.VagaId,
    };

    // Verifica se a moto já está em uma movimentação ativa
    var movAtivaMoto = await _db.Movimentacoes
        .FirstOrDefaultAsync(m => m.MotoId == movimentacao.MotoId && m.DtSaida == null);
    if (movAtivaMoto != null)
    {
        return Results.Conflict("Esta moto já está em uma movimentação ativa.");
    }

    // Verifica se a vaga já está ocupada
    var movAtivaVaga = await _db.Movimentacoes
        .FirstOrDefaultAsync(m => m.VagaId == movimentacao.VagaId && m.DtSaida == null);
    if (movAtivaVaga != null)
    {
        return Results.Conflict("Esta vaga já está ocupada.");
    }
    
    // Procura a moto e a vaga para verificar se existem ou não
    var moto = await _db.Motos
        .Include(m => m.Cliente) 
        .FirstOrDefaultAsync(m => m.MotoId == movimentacao.MotoId);
    
    var vaga = await _db.Vagas
        .Include(v => v.Setor) 
        .FirstOrDefaultAsync(v => v.VagaId == movimentacao.VagaId);
    
    if (moto == null || vaga == null)
    {
        return Results.NotFound("Moto ou vaga não encontrada.");
    }

    // Define a data de entrada e saída (nula)
    movimentacao.DtEntrada = DateTime.Now;
    movimentacao.DtSaida = null;
    
    // Define a situação da moto baseada no setor em que foi estacionada
    string tipoSetor = vaga.Setor.TipoSetor;
    if (new[] { "Pendência", "Sem Placa", "Agendada Para Manutenção" }.Contains(tipoSetor))
    {
        moto.SituacaoMoto = "Inativa";
    }
    else if (new[] { "Reparos Simples", "Danos Estruturais Graves", "Motor Defeituoso" }.Contains(tipoSetor))
    {
        moto.SituacaoMoto = "Manutenção";
    }
    else if (new[] { "Minha Mottu", "Pronta para Aluguel" }.Contains(tipoSetor))
    {
        moto.SituacaoMoto = "Ativa";
    }

    // Atualiza status da vaga
    vaga.StatusOcupada = 1;

    _db.Movimentacoes.Add(movimentacao);
    await _db.SaveChangesAsync();
    
    var movimentacaoDto = MovimentacaoReadDto.ToDto(movimentacao);
    
    int patioId = vaga.Setor.PatioId;
    
    // retorna ao Front os setores atualizados
    var setoresAtualizados = await _db.Setores
        .Where(s => s.PatioId == patioId)
        .Select(s => new
        {
            Setor = s.TipoSetor,
            TotalVagas = _db.Vagas.Count(v => v.SetorId == s.SetorId),
            MotosPresentes = _db.Movimentacoes.Count(m =>
                m.DtSaida == null &&
                _db.Vagas.Where(v => v.SetorId == s.SetorId).Select(v => v.VagaId).Contains(m.VagaId))
        })
        .ToListAsync();
    
    await _hubContext.Clients.Group($"patio-{patioId}")
        .SendAsync("AtualizarOcupacaoTodosSetores", new
        {
            PatioId = patioId,
            Setores = setoresAtualizados
        });
    
    return Results.Created($"/movimentacoes/{movimentacao.MovimentacaoId}", movimentacaoDto);
    }

    public async Task<IResult> UpdateMovimentacaoAsync(int id)
    {
        var movimentacao = await _db.Movimentacoes
            .Include(m => m.Moto)
            .Include(m => m.Vaga)
            .ThenInclude(v => v.Setor)
            .FirstOrDefaultAsync(m => m.MovimentacaoId == id);

        // Verifica se a movimentação existe.
        if (movimentacao is null)
        {
            return Results.NotFound("Movimentação não encontrada.");
        }
    
        // Verifica se a movimentação já foi finalizada
        if (movimentacao.DtSaida != null)
        {
            return Results.BadRequest("Esta movimentação já foi finalizada.");
        }
        

        // Atualiza a data de saída
        movimentacao.DtSaida = DateTime.Now;

        // Atualiza status da vaga para desocupada
        movimentacao.Vaga.StatusOcupada = 0;

        // Atualiza a situação da moto para 'Em Trânsito'
        movimentacao.Moto.SituacaoMoto = "Em Trânsito";
    
        await _db.SaveChangesAsync();
    
        int patioId = movimentacao.Vaga.Setor.PatioId;

        var setoresAtualizados = await _db.Setores
            .Where(s => s.PatioId == patioId)
            .Select(s => new
            {
                Setor = s.TipoSetor,
                TotalVagas = _db.Vagas.Count(v => v.SetorId == s.SetorId),
                MotosPresentes = _db.Movimentacoes.Count(m =>
                    m.DtSaida == null &&
                    _db.Vagas.Where(v => v.SetorId == s.SetorId).Select(v => v.VagaId).Contains(m.VagaId))
            })
            .ToListAsync();
    
        await _hubContext.Clients.Group($"patio-{patioId}")
            .SendAsync("AtualizarOcupacaoTodosSetores", new
            {
                PatioId = patioId,
                Setores = setoresAtualizados
            });

        return Results.NoContent();
    }
}