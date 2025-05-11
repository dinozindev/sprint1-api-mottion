namespace Sprint1_API.Dto;

public record MovimentacaoReadDto(
    int MovimentacaoId,
    DateTime DtEntrada,
    DateTime? DtSaida,
    string? DescricaoMovimentacao,
    MotoReadDto Moto,
    VagaReadDto Vaga);