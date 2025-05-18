namespace Sprint1_API.Dtos;

public record MotoReadDto(
    int MotoId,
    string PlacaMoto,
    string ModeloMoto,
    string SituacaoMoto,
    string ChassiMoto,
    ClienteResumoDto? Cliente);