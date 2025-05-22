using Sprint1_API.Model;

namespace Sprint1_API.Dtos;

public record GerenteReadDto(
    int GerenteId,
    string NomeGerente,
    string TelefoneGerente,
    string CpfGerente,
    PatioResumoDto Patio)
{
    public static GerenteReadDto ToDto(Gerente g) =>
        new(
            g.GerenteId,
            g.NomeGerente,
            g.TelefoneGerente,
            g.CpfGerente,
            PatioResumoDto.ToDto(g.Patio)
        );
};