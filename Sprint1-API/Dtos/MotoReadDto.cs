using Sprint1_API.Model;

namespace Sprint1_API.Dtos;

public record MotoReadDto(
    int MotoId,
    string? PlacaMoto,
    string ModeloMoto,
    string SituacaoMoto,
    string ChassiMoto,
    ClienteResumoDto? Cliente)
{
    public static MotoReadDto ToDto(Moto m) =>
        new(
            m.MotoId,
            m.PlacaMoto,
            m.ModeloMoto,
            m.SituacaoMoto,
            m.ChassiMoto,
            m.Cliente == null 
                ? null 
                : ClienteResumoDto.ToDto(m.Cliente)
        );
};