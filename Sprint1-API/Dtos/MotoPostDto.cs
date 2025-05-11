namespace Sprint1_API.Dto;

public record MotoPostDto(
    string? PlacaMoto,
    string ModeloMoto,
    string SituacaoMoto,
    string ChassiMoto,
    int? ClienteId);