using Sprint1_API.Model;

namespace Sprint1_API.Dtos;

public record VagaResumoDto(
    int VagaId,
    string NumeroVaga,
    int StatusOcupada
    )
{
    public static VagaResumoDto ToDto(Vaga v) =>
        new(
            v.VagaId,
            v.NumeroVaga,
            v.StatusOcupada
        );
};