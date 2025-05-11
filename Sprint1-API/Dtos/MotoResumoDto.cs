namespace Sprint1_API.Dto;

public record MotoResumoDto(
    int MotoId,
    string? PlacaMoto,
    string ModeloMoto,
    string SituacaoMoto,
    string ChassiMoto);