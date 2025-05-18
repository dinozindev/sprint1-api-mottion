namespace Sprint1_API.Dtos;

public record VagaReadDto(
    int VagaId,
    string NumeroVaga,
    int StatusOcupada,
    SetorResumoDto Setor);