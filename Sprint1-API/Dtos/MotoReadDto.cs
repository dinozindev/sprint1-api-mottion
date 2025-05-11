namespace Sprint1_API.Dto;

public record MotoReadDto(
    int MotoId,
    string PlacaMoto,
    string ModeloMoto,
    string SituacaoMoto,
    string ChassiMoto,
    ClienteResumoDto? Cliente);