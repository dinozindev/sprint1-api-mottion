using System.ComponentModel;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Sprint1_API;
using Sprint1_API.Dtos;
using Sprint1_API.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

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

var clientes = app.MapGroup("/clientes").WithTags("Clientes");
var motos = app.MapGroup("/motos").WithTags("Motos");
var patios = app.MapGroup("/patios").WithTags("Patios");
var cargos = app.MapGroup("/cargos").WithTags("Cargos");
var vagas = app.MapGroup("/vagas").WithTags("Vagas");
var movimentacoes = app.MapGroup("/movimentacoes").WithTags("Movimentacoes");
var funcionarios = app.MapGroup("/funcionarios").WithTags("Funcionarios");
var gerentes = app.MapGroup("/gerentes").WithTags("Gerentes");
var setores = app.MapGroup("/setores").WithTags("Setores");



// busca todos os clientes
clientes.MapGet("/", async (AppDbContext db) =>
    {
        var clientesDto = await db.Clientes
            .Include(c => c.Motos)
            .Select(c => new ClienteReadDto(
                c.ClienteId,
                c.NomeCliente,
                c.TelefoneCliente,
                c.SexoCliente,
                c.EmailCliente,
                c.CpfCliente,
                c.Motos.Select(m => new MotoResumoDto(
                    m.MotoId,
                    m.PlacaMoto,
                    m.ModeloMoto,
                    m.SituacaoMoto,
                    m.ChassiMoto
                )).ToList()
            ))
            .ToListAsync();

        return clientesDto.Count == 0 ? Results.NoContent() : Results.Ok(clientesDto);
    })
    .WithSummary("Retorna a lista de todos os clientes.")
    .WithDescription("Retorna a lista de todos os clientes cadastrados no sistema, incluindo também motos associadas a cada um.")
    .Produces<List<ClienteReadDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status500InternalServerError);


// busca cliente pelo ID
clientes.MapGet("/{id}", async ([Description("Identificador único do Cliente")] int id, AppDbContext db) =>
    {
        var cliente = await db.Clientes
            .Include(c => c.Motos)
            .FirstOrDefaultAsync(c => c.ClienteId == id);

        if (cliente == null)
            return Results.NotFound("Cliente não encontrado com o ID fornecido.");

        var clienteDto = new ClienteReadDto(
            cliente.ClienteId,
            cliente.NomeCliente,
            cliente.TelefoneCliente,
            cliente.SexoCliente,
            cliente.EmailCliente,
            cliente.CpfCliente,
            cliente.Motos.Select(m => new MotoResumoDto(
                m.MotoId,
                m.PlacaMoto,
                m.ModeloMoto,
                m.SituacaoMoto,
                m.ChassiMoto
            )).ToList()
        );

        return Results.Ok(clienteDto);
    })
    .WithSummary("Retorna um cliente com sua(s) moto(s) pelo ID")
    .WithDescription("Retorna um cliente e suas motos associadas (caso existam) pelo ID. Retorna 200 OK se o cliente for encontrado, ou erro se não for achado.")
    .Produces<ClienteReadDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);

// retorna todas as motos cadastradas no sistema.
motos.MapGet("/", async (AppDbContext db) =>
    {
        var motos = await db.Motos
            .Include(m => m.Cliente)
            .ToListAsync();

        var motosDto = motos.Select(m => new MotoReadDto(
            m.MotoId,
            m.PlacaMoto,
            m.ModeloMoto,
            m.SituacaoMoto,
            m.ChassiMoto,
            m.Cliente == null 
                ? null 
                : new ClienteResumoDto(
                    m.Cliente.ClienteId,
                    m.Cliente.NomeCliente,
                    m.Cliente.TelefoneCliente,
                    m.Cliente.SexoCliente,
                    m.Cliente.EmailCliente,
                    m.Cliente.CpfCliente
                )
        )).ToList();

        return motosDto.Count == 0 ? Results.NoContent() : Results.Ok(motosDto);
    })
    .WithSummary("Retorna uma lista contendo todas as motos.")
    .WithDescription("Retorna a lista de todas as motos cadastradas no sistema. O id do cliente pode ser nulo ou não.")
    .Produces<List<MotoReadDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status500InternalServerError);


// retorna uma moto através do ID
motos.MapGet("/{id}", async ([Description("Identificador único da Moto")] int id, AppDbContext db) =>
    {
        var moto = await db.Motos
            .Include(m => m.Cliente)
            .FirstOrDefaultAsync(m => m.MotoId == id);

        if (moto == null)
            return Results.NotFound("Moto não encontrada com o ID fornecido.");

        var motoDto = new MotoReadDto(
            moto.MotoId,
            moto.PlacaMoto,
            moto.ModeloMoto,
            moto.SituacaoMoto,
            moto.ChassiMoto,
            moto.Cliente == null 
                ? null 
                : new ClienteResumoDto(
                    moto.Cliente.ClienteId,
                    moto.Cliente.NomeCliente,
                    moto.Cliente.TelefoneCliente,
                    moto.Cliente.SexoCliente,
                    moto.Cliente.EmailCliente,
                    moto.Cliente.CpfCliente
                )
        );

        return Results.Ok(motoDto);
    })
    .WithSummary("Retorna uma moto pelo ID")
    .WithDescription("Retorna uma moto pelo ID. Retorna 200 OK se a moto for encontrada, ou erro se não for achada.")
    .Produces<MotoReadDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);

// retorna uma moto através do número de chassi
motos.MapGet("/por-chassi/{numeroChassi}", async ([Description("Número de Chassi único da Moto")] string numeroChassi, AppDbContext db) =>
    {
        var moto = await db.Motos
            .Include(m => m.Cliente)
            .FirstOrDefaultAsync(m => m.ChassiMoto == numeroChassi);

        if (moto == null)
            return Results.NotFound("Moto não encontrada com o número de chassi fornecido.");

        var motoDto = new MotoReadDto(
            moto.MotoId,
            moto.PlacaMoto,
            moto.ModeloMoto,
            moto.SituacaoMoto,
            moto.ChassiMoto,
            moto.Cliente == null 
                ? null 
                : new ClienteResumoDto(
                    moto.Cliente.ClienteId,
                    moto.Cliente.NomeCliente,
                    moto.Cliente.TelefoneCliente,
                    moto.Cliente.SexoCliente,
                    moto.Cliente.EmailCliente,
                    moto.Cliente.CpfCliente
                )
        );

        return Results.Ok(motoDto);
    })
    .WithSummary("Retorna uma moto pelo Número de Chassi")
    .WithDescription("Retorna uma moto pelo número de Chassi. Retorna 200 OK se a moto for encontrada, ou erro se não for achada.")
    .Produces<MotoReadDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);


// busca a última posição que a moto esteve.
motos.MapGet("/{id}/ultima-posicao", async ([Description("ID único da Moto")] int id, AppDbContext db) =>
    {
        var ultimaMovimentacao = await db.Movimentacoes
            .Where(m => m.MotoId == id)
            .OrderByDescending(m => m.DtEntrada)
            .Include(m => m.Vaga)
            .ThenInclude(v => v.Setor)
            .FirstOrDefaultAsync();

        if (ultimaMovimentacao == null)
            return Results.NotFound("Movimentação não encontrada para essa moto.");

        var ultimaPosicao = new UltimaPosicaoDto(
            new VagaResumoDto(
                ultimaMovimentacao.Vaga.VagaId,
                ultimaMovimentacao.Vaga.NumeroVaga,
                ultimaMovimentacao.Vaga.StatusOcupada
            ),
            new SetorResumoDto(
                ultimaMovimentacao.Vaga.Setor.SetorId,
                ultimaMovimentacao.Vaga.Setor.TipoSetor,
                ultimaMovimentacao.Vaga.Setor.StatusSetor,
                ultimaMovimentacao.Vaga.Setor.PatioId
            ),
            ultimaMovimentacao.DtEntrada,
            ultimaMovimentacao.DtSaida,
            ultimaMovimentacao.DtSaida == null ? true : false
        );

        return Results.Ok(ultimaPosicao);
    })
    .WithSummary("Retorna a última posição da moto")
    .WithDescription("Retorna a última vaga e setor em que a moto esteve, com base na movimentação mais recente.")
    .Produces<UltimaPosicaoDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);

// cria uma nova moto
motos.MapPost("/", async (MotoPostDto dto, AppDbContext db) =>
    {
        var moto = new Moto
        {
            PlacaMoto = dto.PlacaMoto,
            ModeloMoto = dto.ModeloMoto,
            SituacaoMoto = dto.SituacaoMoto,
            ChassiMoto = dto.ChassiMoto,
            ClienteId = null
        };
        
        // verifica se a placa está no formato correto
        if (!string.IsNullOrWhiteSpace(moto.PlacaMoto))
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(moto.PlacaMoto, @"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$"))
            {
                return Results.BadRequest("Placa inválida. Use o formato ABC1234 ou ABC1D23.");
            }

            // verifica se a placa já existe
            var placaExistente = await db.Motos.CountAsync(m => m.PlacaMoto == moto.PlacaMoto);
            if (placaExistente > 0)
                return Results.Conflict($"Já existe uma moto com a placa '{moto.PlacaMoto}'.");
        }


        // verifica se o Chassi está no formato correto
        if (string.IsNullOrWhiteSpace(moto.ChassiMoto) ||
            !System.Text.RegularExpressions.Regex.IsMatch(moto.ChassiMoto, @"^[A-HJ-NPR-Z0-9]{17}$"))
        {
            return Results.BadRequest("Chassi inválido. Deve conter 17 caracteres alfanuméricos, sem I, O ou Q.");
        }

        // verifica se o chassi já existe
        var chassiExistente = await db.Motos.CountAsync(m => m.ChassiMoto == moto.ChassiMoto);
        
        if (chassiExistente > 0)
            return Results.Conflict($"Já existe uma moto com o chassi '{moto.ChassiMoto}'.");
        
        
        var modelosValidos = new[] { "Mottu Pop", "Mottu Sport", "Mottu-E" };
        if (!modelosValidos.Contains(moto.ModeloMoto))
            return Results.BadRequest("Modelo inválido. Os modelos válidos são: Mottu Pop, Mottu Sport, Mottu-E.");
        
        var situacoesValidas = new[] { "Ativa", "Inativa", "Manutenção", "Em Trânsito" };
        if (!situacoesValidas.Contains(moto.SituacaoMoto))
            return Results.BadRequest("Situação inválida. As situações válidas são: Ativa, Inativa, Manutenção.");
        
        db.Motos.Add(moto);
        await db.SaveChangesAsync();
        return Results.Created($"/motos/{moto.MotoId}", moto);
    
})
    .Accepts<MotoPostDto>("application/json")
    .WithSummary("Cria uma moto")
    .WithDescription("Cria uma moto no sistema.")
    .Produces<Moto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status409Conflict)
    .Produces(StatusCodes.Status500InternalServerError);

// atualiza os dados da moto (menos o cliente)
motos.MapPut("/{id}", async ([Description("ID único da Moto")] int id, MotoPostDto dto, AppDbContext db) =>
{
    var motoExistente = await db.Motos.FindAsync(id);
    if (motoExistente is null)
        return Results.NotFound($"Moto com ID {id} não encontrada.");
    
    motoExistente.PlacaMoto = dto.PlacaMoto;
    motoExistente.ModeloMoto = dto.ModeloMoto;
    motoExistente.SituacaoMoto = dto.SituacaoMoto;
    motoExistente.ChassiMoto = dto.ChassiMoto;
    
    if (!string.IsNullOrWhiteSpace(motoExistente.PlacaMoto))
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(motoExistente.PlacaMoto, @"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$"))
        {
            return Results.BadRequest("Placa inválida. Use o formato ABC1234 ou ABC1D23.");
        }

        var placaExistente = await db.Motos
            .CountAsync(m => m.PlacaMoto == motoExistente.PlacaMoto && m.MotoId != id);

        if (placaExistente > 0)
            return Results.Conflict($"Já existe uma moto com a placa '{motoExistente.PlacaMoto}'.");
    }
    
    if (string.IsNullOrWhiteSpace(motoExistente.ChassiMoto) ||
        !System.Text.RegularExpressions.Regex.IsMatch(motoExistente.ChassiMoto, @"^[A-HJ-NPR-Z0-9]{17}$"))
    {
        return Results.BadRequest("Chassi inválido. Deve conter 17 caracteres alfanuméricos, sem I, O ou Q.");
    }

    var chassiExistente = await db.Motos
        .CountAsync(m => m.ChassiMoto == motoExistente.ChassiMoto && m.MotoId != id);

    if (chassiExistente > 0)
        return Results.Conflict($"Já existe uma moto com o chassi '{motoExistente.ChassiMoto}'.");
    
    var modelosValidos = new[] { "Mottu Pop", "Mottu Sport", "Mottu-E" };
    if (!modelosValidos.Contains(motoExistente.ModeloMoto))
        return Results.BadRequest("Modelo inválido. Os modelos válidos são: Mottu Pop, Mottu Sport, Mottu-E.");
    
    var situacoesValidas = new[] { "Ativa", "Inativa", "Manutenção", "Em Trânsito" };
    if (!situacoesValidas.Contains(motoExistente.SituacaoMoto))
        return Results.BadRequest("Situação inválida. As situações válidas são: Ativa, Inativa, Manutenção, Em Trânsito.");
    
    await db.SaveChangesAsync();
    return Results.Ok(motoExistente);

})
.Accepts<MotoPostDto>("application/json")
.WithSummary("Atualiza uma moto")
.WithDescription("Atualiza os dados de uma moto existente.")
.Produces<Moto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status409Conflict)
.Produces(StatusCodes.Status500InternalServerError);


// deleta uma moto pelo ID
motos.MapDelete("/{id}", async ([Description("ID único da Moto")] int id, AppDbContext db) =>
{
    if (await db.Motos.FindAsync(id) is { } existingMoto)
    {
        db.Motos.Remove(existingMoto);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    
    return Results.NotFound("Nenhuma moto encontrada com ID fornecido.");
})
    .WithSummary("Deleta uma moto pelo ID")
    .WithDescription("Retorna uma moto pelo ID informado. Retorna 204 No Content caso encontrado, ou erro se não achado.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);

// remove a associação de uma moto com um cliente
motos.MapPut("/{id}/remover-cliente", async ([Description("ID único da Moto")] int id, AppDbContext db) =>
{
    var moto = await db.Motos.FindAsync(id);
    if (moto is null) return Results.NotFound("Nenhuma moto encontrada com o ID informado.");
    
    moto.ClienteId = null;
    
    await db.SaveChangesAsync();
    
    return Results.NoContent();
    })
    .WithSummary("Remove a associação do cliente a moto.")
    .WithDescription("Remove a associação do cliente de uma moto através do ID da moto.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status500InternalServerError);

// altera a associação de uma moto com um cliente
motos.MapPut("/{id}/alterar-cliente/{clienteId}", async ([Description("ID único da Moto")] int id, [Description("ID único do Cliente")] int clienteId, AppDbContext db) =>
{
    var moto = await db.Motos.FindAsync(id);
    if (moto is null) return Results.NotFound("Nenhuma moto encontrada com o ID informado.");
    
    var cliente = await db.Clientes.FindAsync(clienteId);
    if (cliente is null) return Results.NotFound("Nenhum cliente encontrado com o ID informado.");

    moto.ClienteId = clienteId;
    await db.SaveChangesAsync();
    
    return Results.NoContent();
})
.WithSummary("Altera a associação do cliente a moto.")
.WithDescription("Altera a associação do cliente de uma moto através do ID da moto e do cliente.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.ProducesProblem(StatusCodes.Status500InternalServerError);


//  Retorna todos os Patios cadastrados
patios.MapGet("/", async (AppDbContext db) =>
    {
        var patiosDto = await db.Patios
            .Include(p => p.Setores)
            .ThenInclude(s => s.Vagas)
            .Select(p => new PatioReadDto(
                p.PatioId,
                p.LocalizacaoPatio,
                p.NomePatio,
                p.DescricaoPatio,
                p.Setores.Select(s => new SetorResumoPatioDto(
                    s.SetorId,
                    s.TipoSetor,
                    s.StatusSetor,
                    s.Vagas.Select(v => new VagaResumoDto(
                        v.VagaId,
                        v.NumeroVaga,
                        v.StatusOcupada
                    )).ToList()
                )).ToList()
            )).ToListAsync();

        return patiosDto.Count == 0 ? Results.NoContent() : Results.Ok(patiosDto);
    })
    .WithSummary("Retorna a lista de pátios com setores e vagas")
    .WithDescription("Retorna todos os pátios cadastrados, com seus respectivos setores e vagas.")
    .Produces<List<PatioReadDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status500InternalServerError);
    

// Retorna um patio pelo ID
patios.MapGet("/{id}", async ([Description("Identificador único do patio")] int id, AppDbContext db) =>
    {
    var patio = await db.Patios 
        .Include(p => p.Setores)
        .ThenInclude(s => s.Vagas)
        .FirstOrDefaultAsync(p => p.PatioId == id);

    if (patio is null)
    {
        return Results.NotFound();
    }
    
    var patioDto = new PatioReadDto(
        patio.PatioId,
        patio.LocalizacaoPatio,
        patio.NomePatio,
        patio.DescricaoPatio,
        patio.Setores.Select(s => new SetorResumoPatioDto(
            s.SetorId,
            s.TipoSetor,
            s.StatusSetor,
            s.Vagas.Select(v => new VagaResumoDto(
                v.VagaId,
                v.NumeroVaga,
                v.StatusOcupada
            )).ToList()
        )).ToList()
    );
        
        return Results.Ok(patioDto);
    })
    .WithSummary("Retorna um patio pelo ID")
    .WithDescription("Retorna um patio a partir de um ID. Retorna 200 OK se o patio for encontrado, ou erro se não for achado.")
    .Produces<PatioReadDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);


// Retorna uma lista de todos os cargos
cargos.MapGet("/", async (AppDbContext db) =>
    {
        var cargosDto = await db.Cargos
            .Select(c => new CargoReadDto(
                c.CargoId,
                c.NomeCargo,
                c.DescricaoCargo
            )).ToListAsync();
        
        return cargosDto.Count == 0 ? Results.NoContent() : Results.Ok(cargosDto);
    })
    .WithSummary("Retorna a lista de cargos.")
    .WithDescription("Retorna a lista de cargos cadastrados.")
    .Produces<List<CargoReadDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status500InternalServerError);

// Retorna um cargo a partir do ID
cargos.MapGet("/{id}", async ([Description("Identificador único do cargo")] int id, AppDbContext db) =>
{
    var cargo = await db.Cargos.FirstOrDefaultAsync(c => c.CargoId == id);

    if (cargo is null)
    {
        return Results.NotFound("Nenhum cargo encontrado com o ID fornecido.");
    }

    var cargoDto = new CargoReadDto(
        cargo.CargoId,
        cargo.NomeCargo,
        cargo.DescricaoCargo
    );
    
    return Results.Ok(cargo);
})
    .WithSummary("Retorna um cargo pelo ID")
    .WithDescription("Retorna um cargo a partir de um ID. Retorna 200 OK se o cargo for encontrado, ou erro se não for achado.")
    .Produces<Cargo>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);


// Retorna uma lista de todos os funcionários
funcionarios.MapGet("/", async (AppDbContext db) =>
    {
        var funcionariosDto = await db.Funcionarios
            .Include(f => f.Cargo)
            .Include(f => f.Patio)
            .Select(f => new FuncionarioReadDto(
                f.FuncionarioId,
                f.NomeFuncionario,
                f.TelefoneFuncionario,
                new CargoReadDto(
                    f.Cargo.CargoId,
                    f.Cargo.NomeCargo,
                    f.Cargo.DescricaoCargo
                ),
                new PatioResumoDto(
                    f.Patio.PatioId,
                    f.Patio.LocalizacaoPatio,
                    f.Patio.NomePatio,
                    f.Patio.DescricaoPatio
                )
            ))
            .ToListAsync();

        return funcionariosDto.Count == 0 ? Results.NoContent() : Results.Ok(funcionariosDto);
    })
    .WithSummary("Retorna a lista de funcionários")
    .WithDescription("Retorna a lista de funcionários cadastrados.")
    .Produces<List<FuncionarioReadDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status500InternalServerError);


// Retorna um funcionário a partir do ID
funcionarios.MapGet("/{id}", async ([Description("Identificador único do funcionário")] int id, AppDbContext db) =>
    {
        var funcionario = await db.Funcionarios
            .Include(f => f.Cargo)
            .Include(f => f.Patio)
            .FirstOrDefaultAsync(f => f.FuncionarioId == id);

        if (funcionario == null)
            return Results.NotFound("Nenhum funcionário encontrado com ID fornecido.");

        var funcionarioDto = new FuncionarioReadDto(
            funcionario.FuncionarioId,
            funcionario.NomeFuncionario,
            funcionario.TelefoneFuncionario,
            new CargoReadDto(
                funcionario.Cargo.CargoId,
                funcionario.Cargo.NomeCargo,
                funcionario.Cargo.DescricaoCargo
            ),
            new PatioResumoDto(
                funcionario.Patio.PatioId,
                funcionario.Patio.LocalizacaoPatio,
                funcionario.Patio.NomePatio,
                funcionario.Patio.DescricaoPatio
            )
        );

        return Results.Ok(funcionarioDto);
    })
    .WithSummary("Retorna um funcionário pelo ID")
    .WithDescription("Retorna um funcionário a partir de um ID. Retorna 200 OK se o funcionário for encontrado, ou erro se não for achado.")
    .Produces<FuncionarioReadDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);

// Retorna uma lista de todos os gerentes
gerentes.MapGet("/", async (AppDbContext db) =>
    {
        var gerentesDto = await db.Gerentes
            .Include(g => g.Patio)
            .Select(g => new GerenteReadDto(
                g.GerenteId,
                g.NomeGerente,
                g.TelefoneGerente,
                g.CpfGerente,
                new PatioResumoDto(
                    g.Patio.PatioId,
                    g.Patio.LocalizacaoPatio,
                    g.Patio.NomePatio,
                    g.Patio.DescricaoPatio
                )
            ))
            .ToListAsync();

        return gerentesDto.Count == 0 ? Results.NoContent() : Results.Ok(gerentesDto);
    })
    .WithSummary("Retorna a lista de gerentes")
    .WithDescription("Retorna a lista de gerentes cadastrados.")
    .Produces<List<GerenteReadDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status500InternalServerError);


// Retorna um gerente a partir do ID
gerentes.MapGet("/{id}", async ([Description("Identificador único do gerente")] int id, AppDbContext db) =>
    {
        var gerente = await db.Gerentes
            .Include(g => g.Patio)
            .FirstOrDefaultAsync(g => g.GerenteId == id);

        if (gerente == null)
            return Results.NotFound("Nenhum gerente encontrado com ID fornecido.");

        var gerenteDto = new GerenteReadDto(
            gerente.GerenteId,
            gerente.NomeGerente,
            gerente.TelefoneGerente,
            gerente.CpfGerente,
            new PatioResumoDto(
                gerente.Patio.PatioId,
                gerente.Patio.LocalizacaoPatio,
                gerente.Patio.NomePatio,
                gerente.Patio.DescricaoPatio
            )
        );

        return Results.Ok(gerenteDto);
    })
    .WithSummary("Retorna um gerente pelo ID")
    .WithDescription("Retorna um gerente a partir de um ID. Retorna 200 OK se o gerente for encontrado, ou erro se não for achado.")
    .Produces<GerenteReadDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);


vagas.MapGet("/", async (AppDbContext db) =>
{
    var vagasDto = await db.Vagas
        .Include(v => v.Setor)
        .Select(v => new VagaReadDto(
            v.VagaId,
            v.NumeroVaga,
            v.StatusOcupada,
            new SetorResumoDto(
                v.Setor.SetorId,
                v.Setor.TipoSetor,
                v.Setor.StatusSetor,
                v.Setor.PatioId
                )
            ))
        .ToListAsync();
    
    return vagasDto.Count == 0 ? Results.NoContent() : Results.Ok(vagasDto);
})
    .WithSummary("Retorna a lista de vagas")
    .WithDescription("Retorna a lista de vagas cadastradas.")
    .Produces<List<VagaReadDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status500InternalServerError);

// Retorna uma vaga a partir do ID
vagas.MapGet("/{id}", async ([Description("Identificador único da vaga")] int id, AppDbContext db) =>
{
    var vaga = await db.Vagas
        .Include(v => v.Setor)
        .FirstOrDefaultAsync(v => v.VagaId == id);

    if (vaga == null)
    {
        return Results.NotFound("Nenhuma vaga encontrada com ID fornecido.");
    }

    var vagaDto = new VagaReadDto(
        vaga.VagaId,
        vaga.NumeroVaga,
        vaga.StatusOcupada,
        new SetorResumoDto(
            vaga.Setor.SetorId,
            vaga.Setor.TipoSetor,
            vaga.Setor.StatusSetor,
            vaga.Setor.PatioId
        )
    );
    
    return Results.Ok(vagaDto);
})
    .WithSummary("Retorna uma vaga pelo ID")
    .WithDescription("Retorna uma vaga a partir de um ID. Retorna 200 OK se a vaga for encontrada, ou erro se não for achada.")
    .Produces<Vaga>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);

// Retorna uma lista de todos os setores
setores.MapGet("/", async (AppDbContext db) =>
    {
        var setoresDto = await db.Setores
            .Include(s => s.Patio)
            .Include(s => s.Vagas)
            .Select(s => new SetorReadDto(
                s.SetorId,
                s.TipoSetor,
                s.StatusSetor,
                new PatioResumoDto(
                    s.Patio.PatioId,
                    s.Patio.LocalizacaoPatio,
                    s.Patio.NomePatio,
                    s.Patio.DescricaoPatio
                ),
                s.Vagas.Select(v => new VagaResumoDto(
                    v.VagaId,
                    v.NumeroVaga,
                    v.StatusOcupada)).ToList()
            )).ToListAsync();
        
        return setoresDto.Count == 0 ? Results.NoContent() : Results.Ok(setoresDto);
    })
    .WithSummary("Retorna a lista de setores")
    .WithDescription("Retorna a lista de setores cadastrados.")
    .Produces<List<SetorReadDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status500InternalServerError);


// Retorna um setor a partir do ID
setores.MapGet("/{id}", async ([Description("Identificador único do setor")] int id, AppDbContext db) =>
    {
        var setor = await db.Setores
            .Include(s => s.Patio)
            .Include(s => s.Vagas)
            .FirstOrDefaultAsync(s => s.SetorId == id);

        if (setor == null)
        {
            return Results.NotFound("Nenhum setor encontrado com ID fornecido.");
        }
        
        var setorDto = new SetorReadDto(
                setor.SetorId,
                setor.TipoSetor,
                setor.StatusSetor,
                new PatioResumoDto(
                    setor.Patio.PatioId,
                    setor.Patio.LocalizacaoPatio,
                    setor.Patio.NomePatio,
                    setor.Patio.DescricaoPatio
                ),
                setor.Vagas.Select(v => new VagaResumoDto(
                    v.VagaId,
                    v.NumeroVaga,
                    v.StatusOcupada)).ToList());
        
        return Results.Ok(setorDto);
    })
    .WithSummary("Retorna um setor pelo ID")
    .WithDescription("Retorna um setor a partir de um ID. Retorna 200 OK se o setor for encontrado, ou erro se não for achado.")
    .Produces<SetorReadDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);


// Retorna uma lista de todas as movimentações
movimentacoes.MapGet("/", async (AppDbContext db) =>
    {
        var movimentacoesDto = await db.Movimentacoes
            .Include(m => m.Moto)
            .ThenInclude(mo => mo.Cliente)
            .Include(m => m.Vaga)
            .ThenInclude(v => v.Setor)
            .Select(m => new MovimentacaoReadDto(
                m.MovimentacaoId,
                m.DtEntrada,
                m.DtSaida,
                m.DescricaoMovimentacao,
                new MotoReadDto(
                    m.Moto.MotoId,
                    m.Moto.PlacaMoto,
                    m.Moto.ModeloMoto,
                    m.Moto.SituacaoMoto,
                    m.Moto.ChassiMoto,
                    new ClienteResumoDto(
                        m.Moto.Cliente.ClienteId,
                        m.Moto.Cliente.NomeCliente,
                        m.Moto.Cliente.TelefoneCliente,
                        m.Moto.Cliente.SexoCliente,
                        m.Moto.Cliente.EmailCliente,
                        m.Moto.Cliente.CpfCliente
                    )
                ),
                new VagaReadDto(
                    m.Vaga.VagaId,
                    m.Vaga.NumeroVaga,
                    m.Vaga.StatusOcupada,
                    new SetorResumoDto(
                        m.Vaga.Setor.SetorId,
                        m.Vaga.Setor.TipoSetor,
                        m.Vaga.Setor.StatusSetor,
                        m.Vaga.Setor.PatioId
                    )
                )
            ))
            .ToListAsync();

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
    
    var movimentacoesDto = await db.Movimentacoes
        .Where(m => m.MotoId == motoId)
        .Include(m => m.Moto)
        .ThenInclude(mo => mo.Cliente)
        .Include(m => m.Vaga)
        .ThenInclude(v => v.Setor)
        .Select(m => new MovimentacaoReadDto(
            m.MovimentacaoId,
            m.DtEntrada,
            m.DtSaida,
            m.DescricaoMovimentacao,
            new MotoReadDto(
                m.Moto.MotoId,
                m.Moto.PlacaMoto,
                m.Moto.ModeloMoto,
                m.Moto.SituacaoMoto,
                m.Moto.ChassiMoto,
                new ClienteResumoDto(
                    m.Moto.Cliente.ClienteId,
                    m.Moto.Cliente.NomeCliente,
                    m.Moto.Cliente.TelefoneCliente,
                    m.Moto.Cliente.SexoCliente,
                    m.Moto.Cliente.EmailCliente,
                    m.Moto.Cliente.CpfCliente
                )
            ),
            new VagaReadDto(
                m.Vaga.VagaId,
                m.Vaga.NumeroVaga,
                m.Vaga.StatusOcupada,
                new SetorResumoDto(
                    m.Vaga.Setor.SetorId,
                    m.Vaga.Setor.TipoSetor,
                    m.Vaga.Setor.StatusSetor,
                    m.Vaga.Setor.PatioId
                )
            )
        ))
        .ToListAsync();

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
    
    var movimentacaoDto = new MovimentacaoReadDto(
        movimentacao.MovimentacaoId,
        movimentacao.DtEntrada,
        movimentacao.DtSaida,
        movimentacao.DescricaoMovimentacao,
        new MotoReadDto(
            movimentacao.Moto.MotoId,
            movimentacao.Moto.PlacaMoto,
            movimentacao.Moto.ModeloMoto,
            movimentacao.Moto.SituacaoMoto,
            movimentacao.Moto.ChassiMoto,
            new ClienteResumoDto(
                movimentacao.Moto.Cliente.ClienteId,
                movimentacao.Moto.Cliente.NomeCliente,
                movimentacao.Moto.Cliente.TelefoneCliente,
                movimentacao.Moto.Cliente.SexoCliente,
                movimentacao.Moto.Cliente.EmailCliente,
                movimentacao.Moto.Cliente.CpfCliente
            )
        ),
        new VagaReadDto(
            movimentacao.Vaga.VagaId,
            movimentacao.Vaga.NumeroVaga,
            movimentacao.Vaga.StatusOcupada,
            new SetorResumoDto(
                movimentacao.Vaga.Setor.SetorId,
                movimentacao.Vaga.Setor.TipoSetor,
                movimentacao.Vaga.Setor.StatusSetor,
                movimentacao.Vaga.Setor.PatioId
            )
        )
    );
    
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
    
    
    var movimentacaoDto = new MovimentacaoReadDto(
        movimentacao.MovimentacaoId,
        movimentacao.DtEntrada,
        movimentacao.DtSaida,
        movimentacao.DescricaoMovimentacao,
        new MotoReadDto(
            movimentacao.Moto.MotoId,
            movimentacao.Moto.PlacaMoto,
            movimentacao.Moto.ModeloMoto,
            movimentacao.Moto.SituacaoMoto,
            movimentacao.Moto.ChassiMoto,
            new ClienteResumoDto(
                movimentacao.Moto.Cliente.ClienteId,
                movimentacao.Moto.Cliente.NomeCliente,
                movimentacao.Moto.Cliente.TelefoneCliente,
                movimentacao.Moto.Cliente.SexoCliente,
                movimentacao.Moto.Cliente.EmailCliente,
                movimentacao.Moto.Cliente.CpfCliente
            )
        ),
        new VagaReadDto(
            movimentacao.Vaga.VagaId,
            movimentacao.Vaga.NumeroVaga,
            movimentacao.Vaga.StatusOcupada,
            new SetorResumoDto(
                movimentacao.Vaga.Setor.SetorId,
                movimentacao.Vaga.Setor.TipoSetor,
                movimentacao.Vaga.Setor.StatusSetor,
                movimentacao.Vaga.Setor.PatioId
            )
        )
    );
    
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