namespace Sprint1_API.Dto;

public record PatioReadDto(
    int PatioId,
    string LocalizacaoPatio,
    string NomePatio,
    string DescricaoPatio,
    List<SetorResumoPatioDto> Setores
);