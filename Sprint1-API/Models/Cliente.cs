using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sprint1_API.Model;

[Table("CLIENTE")] 
public class Cliente
{
    [Column("ID_CLIENTE")]
    public int ClienteId { get; set; }

    [Column("TELEFONE_CLIENTE")]
    [StringLength(11)]
    public string TelefoneCliente { get; set; }

    [Column("NOME_CLIENTE")]
    [StringLength(100)]
    public string NomeCliente { get; set; }

    [Column("SEXO_CLIENTE")]
    [StringLength(1)]
    public char SexoCliente { get; set; }

    [Column("EMAIL_CLIENTE")]
    [StringLength(100)]
    public string EmailCliente { get; set; }
    
    [StringLength(11)]
    [Column("CPF_CLIENTE")]
    public string CpfCliente { get; set; }

    public List<Moto> Motos { get; set; }
}