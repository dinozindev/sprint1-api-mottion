namespace Sprint1_API.Dtos;

public record ClienteReadDto(
int ClienteId,
string NomeCliente,
string TelefoneCliente,
char SexoCliente,
string EmailCliente,
string CpfCliente,
List<MotoResumoDto> Motos);