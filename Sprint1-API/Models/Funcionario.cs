using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Sprint1_API.Model;

public class Funcionario
{
    [Column("ID_FUNCIONARIO")]
    public int FuncionarioId { get; set; }
    
    [Column("NOME_FUNCIONARIO")]
    [StringLength(100)]
    public string NomeFuncionario { get; set; }
    
    [Column("TELEFONE_FUNCIONARIO")]
    [StringLength(11)]
    public string TelefoneFuncionario { get; set; }
    
    [Column("CARGO_ID_CARGO")]
    public int CargoId { get; set; }
    
    [Column("PATIO_ID_PATIO")]
    public int PatioId { get; set; }

    [JsonIgnore]
    public Cargo Cargo { get; set; }
    
    [JsonIgnore]
    public Patio Patio { get; set; }
}