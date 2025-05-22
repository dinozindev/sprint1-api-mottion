using Sprint1_API.Model;

namespace Sprint1_API.Dtos;

public record CargoReadDto(
    int CargoId,
    string NomeCargo,
    string DescricaoCargo)
{
    public static CargoReadDto ToDto(Cargo c) =>
        new(
          c.CargoId,
          c.NomeCargo,
          c.DescricaoCargo
            );
};