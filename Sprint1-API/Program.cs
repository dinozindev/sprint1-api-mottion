using System.ComponentModel;
using System.Threading.RateLimiting;
using DotNetEnv;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Sprint1_API;
using Sprint1_API.Dtos;
using Sprint1_API.Endpoints;
using Sprint1_API.Model;
using Sprint1_API.Services;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<MotoService>();
builder.Services.AddScoped<PatioService>();
builder.Services.AddScoped<CargoService>();
builder.Services.AddScoped<FuncionarioService>();
builder.Services.AddScoped<GerenteService>();
builder.Services.AddScoped<VagaService>();
builder.Services.AddScoped<SetorService>();

// define um limite de requisições durante um determinado período.
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
});

builder.Services.AddOpenApi();

// trigga uma exceção caso haja uma.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// adiciona o CORS na aplicação
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(opt =>
    {
        opt.AllowAnyOrigin();
        opt.AllowAnyMethod(); // permite que você faça requisições de qualquer método (GET, POST, PUT, DELETE...)
        opt.AllowAnyHeader();
        opt.WithExposedHeaders("Content-Type", "Accept");
    });
});

builder.Services.AddSignalR();

// usado para poder acessar a API na Azure (Tire o comentário somente se for utilizar na Azure criando a imagem)
//builder.WebHost.UseUrls("http://0.0.0.0:5147");

var app = builder.Build();

//  habilita o CORS
app.UseCors();
// limita a qtnd de requisições
app.UseRateLimiter();

app.MapHub<SetorHub>("/hub/setores");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var movimentacoes = app.MapGroup("/movimentacoes").WithTags("Movimentacoes");

// endpoints de Cliente
app.MapClienteEndpoints();

// endpoints de Moto
app.MapMotoEndpoints();

// endpoints de Pátio
app.MapPatioEndpoints();

// endpoints de Cargo
app.MapCargoEndpoints();

// endpoints de Funcionário
app.MapFuncionarioEndpoints();

// endpoints de Gerente
app.MapGerenteEndpoints();

// endpoints de Vaga
app.MapVagaEndpoints();

// endpoints de Setor
app.MapSetorEndpoints();

// Retorna uma lista de todas as movimentações
movimentacoes.MapGet("/", async (AppDbContext db) =>
{
    var movimentacoesObtidas = await db.Movimentacoes
        .Include(m => m.Moto)
        .ThenInclude(mo => mo.Cliente)
        .Include(m => m.Vaga)
        .ThenInclude(v => v.Setor)
        .ToListAsync();

    var movimentacoesDto = movimentacoesObtidas
        .Select(MovimentacaoReadDto.ToDto)
        .ToList();

    return movimentacoesDto.Count == 0 ? Results.NoContent() : Results.Ok(movimentacoesDto);
})
    .WithSummary("Retorna a lista de movimentações")
    .WithDescription("Retorna a lista de movimentações feitas, com dados da moto, cliente e vaga.")
    .Produces<List<MovimentacaoReadDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status500InternalServerError);


// Retorna todas as movimentações de uma moto específica pelo ID
movimentacoes.MapGet("/por-moto/{motoId}", async ([Description("ID único da Moto")] int motoId, AppDbContext db) =>
{
    // Verifica se a moto existe
    var moto = await db.Motos
        .Where(m => m.MotoId == motoId)
        .FirstOrDefaultAsync();

    if (moto == null)
        return Results.NotFound("Moto não encontrada.");
    
    var movimentacoesObtidas = await db.Movimentacoes
        .Where(m => m.MotoId == motoId)
        .Include(m => m.Moto)
        .ThenInclude(mo => mo.Cliente)
        .Include(m => m.Vaga)
        .ThenInclude(v => v.Setor)
        .ToListAsync();

    var movimentacoesDto = movimentacoesObtidas
        .Select(MovimentacaoReadDto.ToDto)
        .ToList();

    return movimentacoesDto.Count == 0 ? Results.NoContent() : Results.Ok(movimentacoesDto);
})
.WithSummary("Retorna movimentações de uma moto específica")
.WithDescription("Retorna a lista de movimentações associadas a uma moto.")
.Produces<List<MovimentacaoReadDto>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);



// Retorna uma movimentação pelo ID
movimentacoes.MapGet("/{id}", async ([Description("Identificador único de movimentação")] int id, AppDbContext db) =>
{
    var movimentacao = await db.Movimentacoes
        .Include(m => m.Moto)
            .ThenInclude(m => m.Cliente)
        .Include(m => m.Vaga)
            .ThenInclude(v => v.Setor)
            .ThenInclude(s => s.Patio)
        .FirstOrDefaultAsync(s => s.MovimentacaoId == id);
    
    if (movimentacao == null)
    {
        return Results.NotFound("Nenhuma movimentação encontrada com o ID fornecido.");  
    }
    
    var movimentacaoDto = MovimentacaoReadDto.ToDto(movimentacao);
    
    return Results.Ok(movimentacaoDto); 
})
.WithSummary("Retorna uma movimentação pelo ID")
.WithDescription("Retorna uma movimentação a partir de um ID. Retorna 200 OK se a movimentação for encontrada, ou erro se não for achada.")
.Produces<MovimentacaoReadDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);


// Retorna a quantidade de vagas ocupadas e o total de vagas por Setor de um Patio
movimentacoes.MapGet("/ocupacao-por-setor/patio/{id}", async ([Description("ID único do Pátio")] int id, AppDbContext db) =>
{
    var resultado = await db.Setores
        .Where(s => s.PatioId == id)
        .Select(s => new
        {
            Setor = s.TipoSetor,
            TotalVagas = db.Vagas.Count(v => v.SetorId == s.SetorId),
            MotosPresentes = db.Movimentacoes.Count(m =>
                m.DtSaida == null &&
                db.Vagas
                    .Where(v => v.SetorId == s.SetorId)
                    .Select(v => v.VagaId)
                    .Contains(m.VagaId)
            )
        })
        .ToListAsync();

    return Results.Ok(resultado);
})
    .WithSummary("Retorna o total de vagas por setor")
    .WithDescription("Retorna o total de vagas e o total de vagas ocupadas por setor a partir do ID de um pátio.")
    .Produces<List<VagasSetorDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status500InternalServerError);


// cria uma nova movimentação
movimentacoes.MapPost("/", async (MovimentacaoPostDto dto, AppDbContext db, IHubContext<SetorHub> hub) =>
{
    var movimentacao = new Movimentacao
    {
        DescricaoMovimentacao = dto.DescricaoMovimentacao,
        MotoId = dto.MotoId,
        VagaId = dto.VagaId,
    };

    // Verifica se a moto já está em uma movimentação ativa
    var movAtivaMoto = await db.Movimentacoes
        .FirstOrDefaultAsync(m => m.MotoId == movimentacao.MotoId && m.DtSaida == null);
    if (movAtivaMoto != null)
    {
        return Results.Conflict("Esta moto já está em uma movimentação ativa.");
    }

    // Verifica se a vaga já está ocupada
    var movAtivaVaga = await db.Movimentacoes
        .FirstOrDefaultAsync(m => m.VagaId == movimentacao.VagaId && m.DtSaida == null);
    if (movAtivaVaga != null)
    {
        return Results.Conflict("Esta vaga já está ocupada.");
    }
    
    // Procura a moto e a vaga para verificar se existem ou não
    var moto = await db.Motos
        .Include(m => m.Cliente) 
        .FirstOrDefaultAsync(m => m.MotoId == movimentacao.MotoId);
    
    var vaga = await db.Vagas
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

    db.Movimentacoes.Add(movimentacao);
    await db.SaveChangesAsync();
    
    var movimentacaoDto = MovimentacaoReadDto.ToDto(movimentacao);
    
    int patioId = vaga.Setor.PatioId;
    
    // retorna ao Front os setores atualizados
    var setoresAtualizados = await db.Setores
        .Where(s => s.PatioId == patioId)
        .Select(s => new
        {
            Setor = s.TipoSetor,
            TotalVagas = db.Vagas.Count(v => v.SetorId == s.SetorId),
            MotosPresentes = db.Movimentacoes.Count(m =>
                m.DtSaida == null &&
                db.Vagas.Where(v => v.SetorId == s.SetorId).Select(v => v.VagaId).Contains(m.VagaId))
        })
        .ToListAsync();
    
    await hub.Clients.Group($"patio-{patioId}")
        .SendAsync("AtualizarOcupacaoTodosSetores", new
        {
            PatioId = patioId,
            Setores = setoresAtualizados
        });
    
    
    return Results.Created($"/movimentacoes/{movimentacao.MovimentacaoId}", movimentacaoDto);
})
.Accepts<MovimentacaoPostDto>("application/json")
.WithSummary("Cria uma nova movimentação")
.WithDescription("Cria uma nova movimentação no sistema, atualizando o status da moto e o status da vaga.")
.Produces<MovimentacaoReadDto>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status409Conflict)
.Produces(StatusCodes.Status500InternalServerError);


// atualiza a data de saída de uma movimentação
movimentacoes.MapPut("/{id}/saida", async ([Description("ID único da Movimentação")] int id, AppDbContext db, IHubContext<SetorHub> hub) =>
{
    var movimentacao = await db.Movimentacoes
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
    
    await db.SaveChangesAsync();
    
    int patioId = movimentacao.Vaga.Setor.PatioId;

    var setoresAtualizados = await db.Setores
        .Where(s => s.PatioId == patioId)
        .Select(s => new
        {
            Setor = s.TipoSetor,
            TotalVagas = db.Vagas.Count(v => v.SetorId == s.SetorId),
            MotosPresentes = db.Movimentacoes.Count(m =>
                m.DtSaida == null &&
                db.Vagas.Where(v => v.SetorId == s.SetorId).Select(v => v.VagaId).Contains(m.VagaId))
        })
        .ToListAsync();
    
    await hub.Clients.Group($"patio-{patioId}")
        .SendAsync("AtualizarOcupacaoTodosSetores", new
        {
            PatioId = patioId,
            Setores = setoresAtualizados
        });

    return Results.NoContent();
})
.WithSummary("Atualiza a data de saída da movimentação.")
.WithDescription("Altera a data de saída de uma movimentação, finalizando-a. Atualiza a situação da moto para 'Em Trânsito' e desocupa a vaga.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound)
.ProducesProblem(StatusCodes.Status500InternalServerError);

app.Run();