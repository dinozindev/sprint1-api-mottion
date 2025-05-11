using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Sprint1_API.Model;

public class Movimentacao
{
    [Column("ID_MOVIMENTACAO")]
    public int MovimentacaoId { get; set; }
    
    [Column("DT_ENTRADA")]
    public DateTime DtEntrada { get; set; }
    
    [Column("DT_SAIDA")]
    public DateTime? DtSaida { get; set; }
    
    [Column("DESCRICAO_MOVIMENTACAO")]
    [StringLength(255)]
    public string? DescricaoMovimentacao { get; set; }
    
    [Column("MOTO_ID_MOTO")]
    public int MotoId { get; set; }
    
    [Column("VAGA_ID_VAGA")]
    public int VagaId { get; set; }
    
    [JsonIgnore]
    public Moto Moto { get; set; }
    [JsonIgnore]
    public Vaga Vaga { get; set; }
}