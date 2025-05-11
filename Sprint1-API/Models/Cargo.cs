using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint1_API.Model;

public class Cargo
{
    [Column("ID_CARGO")]
    public int CargoId { get; set; }
    
    [Column("NOME_CARGO")]
    [StringLength(50)]
    public string NomeCargo { get; set; }
    
    [Column("DESCRICAO_CARGO")]
    [StringLength(255)]
    public string DescricaoCargo { get; set; }
}