using Sprint1_API.Model;

namespace Sprint1_API.Dtos;

public record SetorResumoPatioDto(
    int SetorId,
    string TipoSetor,
    string StatusSetor,
    List<VagaResumoDto> Vagas
)
{
    public static SetorResumoPatioDto ToDto(Setor s) =>
        new(
            s.SetorId,
            s.TipoSetor,
            s.StatusSetor,
            s.Vagas.Select(VagaResumoDto.ToDto).ToList()
            );
};