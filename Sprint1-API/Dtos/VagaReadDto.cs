namespace Sprint1_API.Dto;

public record VagaReadDto(
    int VagaId,
    string NumeroVaga,
    int StatusOcupada,
    SetorResumoDto Setor);