using Sprint1_API.Model;

namespace Sprint1_API.Dtos;

public record MotoResumoDto(
    int MotoId,
    string? PlacaMoto,
    string ModeloMoto,
    string SituacaoMoto,
    string ChassiMoto)
{
    public static MotoResumoDto ToDto(Moto m) =>
        new(
            m.MotoId,
            m.PlacaMoto,
            m.ModeloMoto,
            m.SituacaoMoto,
            m.ChassiMoto
            );
};