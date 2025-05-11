namespace Sprint1_API.Dto;

public record GerenteReadDto(
    int GerenteId,
    string NomeGerente,
    string TelefoneGerente,
    string CpfGerente,
    PatioResumoDto Patio);