namespace Sprint1_API.Dto;

public record MovimentacaoPostDto(
    string DescricaoMovimentacao,
    int MotoId,
    int VagaId);