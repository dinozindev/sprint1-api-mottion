using Sprint1_API.Model;

namespace Sprint1_API.Dtos;

public record ClienteReadDto(
    int ClienteId,
    string NomeCliente,
    string TelefoneCliente,
    char SexoCliente,
    string EmailCliente,
    string CpfCliente,
    List<MotoResumoDto> Motos)
{
    public static ClienteReadDto ToDto(Cliente c) =>
        new(
            c.ClienteId,
            c.NomeCliente,
            c.TelefoneCliente,
            c.SexoCliente,
            c.EmailCliente,
            c.CpfCliente,
            c.Motos.Select(MotoResumoDto.ToDto).ToList()
        );
};