using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Sprint1_API.Model;

[Table("MOTO")]
public class Moto
{
    [Column("ID_MOTO")]
    public int MotoId { get; set; }

    [StringLength(7)]
    [Column("PLACA_MOTO")]
    public string? PlacaMoto { get; set; }
    
    [StringLength(70)]
    [Column("MODELO_MOTO")]
    public string ModeloMoto { get; set; }
    
    [StringLength(50)]
    [Column("SITUACAO_MOTO")]
    public string SituacaoMoto { get; set; }
    
    [StringLength(17)]
    [Column("CHASSI_MOTO")]
    public string ChassiMoto { get; set; }

    [Column("CLIENTE_ID_CLIENTE")]
    public int? ClienteId { get; set; }

    [JsonIgnore]
    public Cliente? Cliente { get; set; }
}