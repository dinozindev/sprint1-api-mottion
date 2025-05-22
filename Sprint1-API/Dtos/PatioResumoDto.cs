using Sprint1_API.Model;

namespace Sprint1_API.Dtos;

public record PatioResumoDto(
    int PatioId,
    string LocalizacaoPatio,
    string NomePatio,
    string DescricaoPatio)
{
    public static PatioResumoDto ToDto(Patio p) =>
        new(
            p.PatioId,
            p.LocalizacaoPatio,
            p.NomePatio,
            p.DescricaoPatio
        );
};