using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Sprint1_API.Model;

public class Gerente
{
    [Column("ID_GERENTE")]
    public int GerenteId { get; set; }
    
    [Column("NOME_GERENTE")]
    [StringLength(100)]
    public string NomeGerente { get; set; }
    
    [Column("TELEFONE_GERENTE")]
    [StringLength(11)]
    public string TelefoneGerente { get; set; }
    
    [Column("CPF_GERENTE")]
    [StringLength(11)]
    public string CpfGerente { get; set; }
    
    [Column("PATIO_ID_PATIO")]
    public int PatioId { get; set; }
    
    [JsonIgnore]
    public Patio Patio { get; set; }
}