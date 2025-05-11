using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Sprint1_API.Model;

public class Vaga
{
    [Column("ID_VAGA")]
    public int VagaId { get; set; }
    
    [Column("NUMERO_VAGA")]
    [StringLength(10)]
    public string NumeroVaga { get; set; }
    
    [Column("STATUS_OCUPADA")]
    public int StatusOcupada { get; set; }
    
    
    [Column("SETOR_ID_SETOR")]
    public int SetorId { get; set; }
    
    [JsonIgnore]
    public Setor Setor { get; set; }
}