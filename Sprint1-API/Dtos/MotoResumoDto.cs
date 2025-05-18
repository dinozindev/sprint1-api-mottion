namespace Sprint1_API.Dtos;

public record MotoResumoDto(
    int MotoId,
    string? PlacaMoto,
    string ModeloMoto,
    string SituacaoMoto,
    string ChassiMoto);