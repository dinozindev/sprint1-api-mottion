namespace Sprint1_API.Dto;

public record SetorReadDto(
    int SetorId,
    string TipoSetor,
    string StatusSetor,
    PatioResumoDto Patio,
    List<VagaResumoDto> Vagas
    );