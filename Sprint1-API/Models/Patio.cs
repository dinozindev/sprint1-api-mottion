using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint1_API.Model;

public class Patio
{
    [Column("ID_PATIO")]
    public int PatioId { get; set; }
    
    [Column("LOCALIZACAO_PATIO")]
    [StringLength(100)]
    public string LocalizacaoPatio { get; set; }
    
    [Column("NOME_PATIO")]
    [StringLength(100)]
    public string NomePatio { get; set; }
    
    [Column("DESCRICAO_PATIO")]
    [StringLength(255)]
    public string DescricaoPatio { get; set; }
    
    public List<Setor> Setores { get; set; }
}