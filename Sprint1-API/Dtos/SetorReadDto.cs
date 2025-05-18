namespace Sprint1_API.Dtos;

public record SetorReadDto(
    int SetorId,
    string TipoSetor,
    string StatusSetor,
    PatioResumoDto Patio,
    List<VagaResumoDto> Vagas
    );