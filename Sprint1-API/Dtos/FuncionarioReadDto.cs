namespace Sprint1_API.Dtos;

public record FuncionarioReadDto(
    int FuncionarioId,
    string NomeFuncionario,
    string TelefoneFuncionario,
    CargoReadDto Cargo,
    PatioResumoDto Patio);