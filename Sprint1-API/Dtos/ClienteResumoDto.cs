namespace Sprint1_API.Dto;

public record ClienteResumoDto(
    int ClienteId,
    string NomeCliente,
    string TelefoneCliente,
    char SexoCliente,
    string EmailCliente,
    string CpfCliente);