namespace Sprint1_API.Dto;

public record FuncionarioReadDto(
    int FuncionarioId,
    string NomeFuncionario,
    string TelefoneFuncionario,
    CargoReadDto Cargo,
    PatioResumoDto Patio);