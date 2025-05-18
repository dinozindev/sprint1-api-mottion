namespace Sprint1_API.Dtos;

public record GerenteReadDto(
    int GerenteId,
    string NomeGerente,
    string TelefoneGerente,
    string CpfGerente,
    PatioResumoDto Patio);