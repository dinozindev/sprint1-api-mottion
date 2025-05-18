namespace Sprint1_API.Dtos;

public record ClienteResumoDto(
    int ClienteId,
    string NomeCliente,
    string TelefoneCliente,
    char SexoCliente,
    string EmailCliente,
    string CpfCliente);