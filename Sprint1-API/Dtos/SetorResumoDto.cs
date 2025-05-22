using Sprint1_API.Model;

namespace Sprint1_API.Dtos;

public record SetorResumoDto(
    int SetorId,
    string TipoSetor,
    string StatusSetor,
    int PatioId)
{
    public static SetorResumoDto ToDto(Setor s) =>
        new(
            s.SetorId,
            s.TipoSetor,
            s.StatusSetor,
            s.PatioId
        );
}