namespace Sprint1_API.Dto;

public record UltimaPosicaoDto(
    VagaResumoDto Vaga,
    SetorResumoDto Setor,
    DateTime DtEntrada,
    DateTime? DtSaida,
    bool Permanece
    );