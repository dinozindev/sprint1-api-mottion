namespace Sprint1_API.Dtos;

public record MovimentacaoReadDto(
    int MovimentacaoId,
    DateTime DtEntrada,
    DateTime? DtSaida,
    string? DescricaoMovimentacao,
    MotoReadDto Moto,
    VagaReadDto Vaga);