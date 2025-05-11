namespace Sprint1_API.Dto;

public record SetorResumoPatioDto(
    int SetorId,
    string TipoSetor,
    string StatusSetor,
    List<VagaResumoDto> Vagas
    );