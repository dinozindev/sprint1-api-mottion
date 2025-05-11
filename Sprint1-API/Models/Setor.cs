using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Sprint1_API.Model;

public class Setor
{
    [Column("ID_SETOR")]
    public int SetorId { get; set; }
    
    [Column("TIPO_SETOR")]
    [StringLength(70)]
    public string TipoSetor { get; set; }
    
    [Column("STATUS_SETOR")]
    [StringLength(50)]
    public string StatusSetor { get; set; }
    
    [Column("PATIO_ID_PATIO")]
    public int PatioId { get; set; }
    
    [JsonIgnore]
    public Patio Patio { get; set; }
    
    public List<Vaga> Vagas { get; set; }
}