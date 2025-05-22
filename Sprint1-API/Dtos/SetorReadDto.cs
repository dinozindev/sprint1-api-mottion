using Sprint1_API.Model;

namespace Sprint1_API.Dtos;

public record SetorReadDto(
    int SetorId,
    string TipoSetor,
    string StatusSetor,
    PatioResumoDto Patio,
    List<VagaResumoDto> Vagas
    )
{
    public static SetorReadDto ToDto(Setor s) =>
        new(
            s.SetorId,
            s.TipoSetor,
            s.StatusSetor,
            PatioResumoDto.ToDto(s.Patio),
            s.Vagas.Select(VagaResumoDto.ToDto).ToList()
        );
};