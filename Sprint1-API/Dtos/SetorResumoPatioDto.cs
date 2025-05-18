namespace Sprint1_API.Dtos;

public record SetorResumoPatioDto(
    int SetorId,
    string TipoSetor,
    string StatusSetor,
    List<VagaResumoDto> Vagas
    );