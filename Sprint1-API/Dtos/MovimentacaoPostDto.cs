namespace Sprint1_API.Dtos;

public record MovimentacaoPostDto(
    string DescricaoMovimentacao,
    int MotoId,
    int VagaId);