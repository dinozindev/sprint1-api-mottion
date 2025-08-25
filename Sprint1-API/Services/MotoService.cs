using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Sprache;
using Sprint1_API.Dtos;
using Sprint1_API.Model;

namespace Sprint1_API.Services;

public class MotoService
{
    private readonly AppDbContext _db;

    public MotoService(AppDbContext db)
    {
        _db = db;
    }

    // retorna todas as motos
    public async Task<IResult> GetAllMotosAsync()
    {
        var motos = await _db.Motos
            .Include(m => m.ClienteId)
            .ToListAsync();
        var motosDto = motos.Select(MotoReadDto.ToDto).ToList();
        return motosDto.Any() ? Results.Ok(motosDto) : Results.NoContent();
    } 
    
    // retorna a moto pelo ID
    public async Task<IResult> GetMotoByIdAsync(int id)
    {
        var moto = await _db.Motos
            .Include(m => m.Cliente)
            .FirstOrDefaultAsync(m => m.MotoId == id);

        return moto is null
            ? Results.NotFound("Nenhuma Moto encontrada com o ID informado.")
            : Results.Ok(MotoReadDto.ToDto(moto));
    }
    
    // retorna uma moto pelo número de Chassi
    public async Task<IResult> GetMotoByChassiAsync(string numeroChassi)
    {
        var moto = await _db.Motos
            .Include(m => m.Cliente)
            .FirstOrDefaultAsync(m => m.ChassiMoto == numeroChassi);

        return moto is null
            ? Results.NotFound("Nenhuma Moto encontrada com o número de chassi informado.")
            : Results.Ok(MotoReadDto.ToDto(moto));
    }
    
    // busca a ultima posição que a moto esteve
    public async Task<IResult> GetMotoUltimaPosicaoAsync(int id)
    {
        var ultimaMovimentacao = await _db.Movimentacoes
            .Where(m => m.MotoId == id) 
            .OrderByDescending(m => m.DtEntrada)
            .Include(m => m.Vaga)
            .ThenInclude(v => v.Setor)
            .FirstOrDefaultAsync();

        if (ultimaMovimentacao is null) return Results.NotFound("Movimentação não encontrada para essa moto.");

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
            ultimaMovimentacao.DtSaida is null
        );

        return Results.Ok(ultimaPosicao);
    }

    // cria uma moto
    public async Task<IResult> CreateMotoAsync(MotoPostDto dto)
    {
        var validation = await ValidateMoto(dto);
        if (validation is not null) return validation;
        
        var moto = new Moto()
        {
            PlacaMoto = dto.PlacaMoto,
            ModeloMoto = dto.ModeloMoto,
            SituacaoMoto = dto.SituacaoMoto,
            ChassiMoto = dto.ChassiMoto,
            ClienteId = null
        };
        
        _db.Motos.Add(moto);
        await _db.SaveChangesAsync();
        return Results.Created($"/motos/{moto.MotoId}", moto);
    }

    // atualiza os dados de uma moto
    public async Task<IResult> UpdateMotoAsync(int id, MotoPostDto dto)
    {
        var motoExistente = await _db.Motos.FindAsync(id);
        if (motoExistente is null) return Results.NotFound("Moto não encontrada com ID informado.");
        
        var validation = await ValidateMoto(dto);
        if (validation is not null) return validation;
        
        motoExistente.PlacaMoto = dto.PlacaMoto;
        motoExistente.ModeloMoto = dto.ModeloMoto;
        motoExistente.SituacaoMoto = dto.SituacaoMoto;
        motoExistente.ChassiMoto = dto.ChassiMoto;

        await _db.SaveChangesAsync();
        return Results.Ok(motoExistente);
    }
    
    // deleta uma moto pelo ID
    public async Task<IResult> DeleteMotoAsync(int id)
    {
        var moto = await _db.Motos.FindAsync(id);
        if (moto is null) return Results.NotFound("Moto não encontrada com ID informado.");
        
        _db.Motos.Remove(moto);
        await _db.SaveChangesAsync();
        return Results.NoContent();
    }

    // remove associação de Cliente de uma Moto e depois atualiza
    public async Task<IResult> DeleteAssociacaoClienteMotoAsync(int id)
    {
        var moto = await _db.Motos.FindAsync(id);
        if (moto is null) return Results.NotFound("Nenhuma moto encontrada com o ID informado.");

        moto.ClienteId = null;

        await _db.SaveChangesAsync();
        
        return Results.NoContent();
    }
    
    // atualiza a associação de uma moto com um cliente
    public async Task<IResult> UpdateAssociacaoClienteMotoAsync(int idMoto, int idCliente)
    {
        var moto = await _db.Motos.FindAsync(idMoto);
        if (moto is null) return Results.NotFound("Nenhuma moto encontrada com o ID informado.");
        
        var cliente = await _db.Clientes.FindAsync(idCliente);
        if (cliente is null) return Results.NotFound("Nenhum cliente encontrado com o ID informado.");
        
        moto.ClienteId = idCliente;
        await _db.SaveChangesAsync();

        return Results.NoContent();
    }

    private async Task<IResult?> ValidateMoto(MotoPostDto dto, int? ignoreId = null)
    {
        // verifica se a placa está no formato correto
        if (!string.IsNullOrWhiteSpace(dto.PlacaMoto))
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(dto.PlacaMoto, @"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$", RegexOptions.None, TimeSpan.FromMilliseconds(100)))
            {
                return Results.BadRequest("Placa inválida. Use o formato ABC1234 ou ABC1D23.");
            }

            // verifica se a placa já existe
            var placaExistente = await _db.Motos
                .AnyAsync(m => m.PlacaMoto == dto.PlacaMoto 
                    && (!ignoreId.HasValue || m.MotoId != ignoreId.Value));
            if (placaExistente)
                return Results.Conflict($"Já existe uma moto com a placa '{dto.PlacaMoto}'.");
        }
        
        // verifica se o Chassi está no formato correto
        if (string.IsNullOrWhiteSpace(dto.ChassiMoto) ||
            !System.Text.RegularExpressions.Regex.IsMatch(dto.ChassiMoto, @"^[A-HJ-NPR-Z0-9]{17}$", RegexOptions.None, TimeSpan.FromMilliseconds(100)))
        {
            return Results.BadRequest("Chassi inválido. Deve conter 17 caracteres alfanuméricos, sem I, O ou Q.");
        }

        // verifica se o chassi já existe
        var chassiExistente = await _db.Motos
            .AnyAsync(m => m.ChassiMoto == dto.ChassiMoto 
                           && (!ignoreId.HasValue || m.MotoId != ignoreId.Value));
        if (chassiExistente)
            return Results.Conflict($"Já existe uma moto com o chassi '{dto.ChassiMoto}'.");
        
        var modelosValidos = new[] { "Mottu Pop", "Mottu Sport", "Mottu-E" };
        if (!modelosValidos.Contains(dto.ModeloMoto))
            return Results.BadRequest("Modelo inválido. Os modelos válidos são: Mottu Pop, Mottu Sport, Mottu-E.");
        
        var situacoesValidas = new[] { "Ativa", "Inativa", "Manutenção", "Em Trânsito" };
        if (!situacoesValidas.Contains(dto.SituacaoMoto))
            return Results.BadRequest("Situação inválida. As situações válidas são: Ativa, Inativa, Manutenção.");

        return null;
    } 
}