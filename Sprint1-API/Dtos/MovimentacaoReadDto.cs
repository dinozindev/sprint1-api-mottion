using Sprint1_API.Model;

namespace Sprint1_API.Dtos;

public record MovimentacaoReadDto(
    int MovimentacaoId,
    DateTime DtEntrada,
    DateTime? DtSaida,
    string? DescricaoMovimentacao,
    MotoReadDto Moto,
    VagaReadDto Vaga)
{
    public static MovimentacaoReadDto ToDto(Movimentacao m) => 
        new(
            m.MovimentacaoId,
            m.DtEntrada,
            m.DtSaida,
            m.DescricaoMovimentacao,
            MotoReadDto.ToDto(m.Moto),
            VagaReadDto.ToDto(m.Vaga)
        );
};