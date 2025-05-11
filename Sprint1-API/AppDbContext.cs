using Microsoft.EntityFrameworkCore;
using Sprint1_API.Model;

namespace Sprint1_API;

public class AppDbContext : DbContext
{
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Moto> Motos { get; set; }
    public DbSet<Patio> Patios { get; set; }
    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<Setor> Setores { get; set; }
    public DbSet<Vaga> Vagas { get; set; }
    public DbSet<Movimentacao> Movimentacoes { get; set; }
    public DbSet<Funcionario> Funcionarios { get; set; }
    public DbSet<Gerente> Gerentes { get; set; }
    
    public String DbPath { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Cliente>()
        .ToTable("CLIENTE")
        .HasMany(c => c.Motos)
        .WithOne(m => m.Cliente)
        .HasForeignKey(m => m.ClienteId)
        .OnDelete(DeleteBehavior.SetNull);
    
    modelBuilder.Entity<Cliente>()
        .HasIndex(c => c.CpfCliente)
        .IsUnique();
    
    modelBuilder.Entity<Moto>()
        .ToTable("MOTO");
    
    modelBuilder.Entity<Moto>()
        .HasIndex(m => m.PlacaMoto)
        .IsUnique();
    
    modelBuilder.Entity<Moto>()
        .HasIndex(m => m.ChassiMoto)
        .IsUnique();
    
    modelBuilder.Entity<Moto>()
        .HasCheckConstraint("CHK_MODELO_MOTO", "MODELO_MOTO IN ('Mottu Pop', 'Mottu Sport', 'Mottu-E')");

    modelBuilder.Entity<Moto>()
        .HasCheckConstraint("CHK_SITUACAO_MOTO", "SITUACAO_MOTO IN ('Inativa', 'Ativa', 'Manutenção', 'Em Trânsito')");

    modelBuilder.Entity<Patio>()
        .ToTable("PATIO")
        .HasMany(p => p.Setores)
        .WithOne(s => s.Patio)
        .HasForeignKey(s => s.PatioId)
        .OnDelete(DeleteBehavior.Cascade);
    
    modelBuilder.Entity<Cargo>()
        .ToTable("CARGO");
    
    modelBuilder.Entity<Setor>()
        .ToTable("SETOR")
        .HasMany(s => s.Vagas)
        .WithOne(v => v.Setor)
        .HasForeignKey(v => v.SetorId)
        .OnDelete(DeleteBehavior.Cascade);
    
    modelBuilder.Entity<Setor>()
        .HasCheckConstraint("CHK_TIPO_SETOR", "TIPO_SETOR IN ('Pendência', 'Reparos Simples', 'Danos Estruturais Graves', 'Motor Defeituoso', 'Agendada Para Manutenção', 'Pronta para Aluguel', 'Sem Placa', 'Minha Mottu')");

    modelBuilder.Entity<Setor>()
        .HasCheckConstraint("CHK_STATUS_SETOR", "STATUS_SETOR IN ('Cheia', 'Parcial', 'Livre')");
    
    modelBuilder.Entity<Vaga>()
        .ToTable("VAGA");
    
    modelBuilder.Entity<Movimentacao>()
        .ToTable("MOVIMENTACAO");
    
    modelBuilder.Entity<Movimentacao>()
        .HasOne(m => m.Moto)
        .WithMany() 
        .HasForeignKey(m => m.MotoId)
        .OnDelete(DeleteBehavior.Cascade);  
    
    modelBuilder.Entity<Movimentacao>()
        .HasOne(m => m.Vaga)
        .WithMany() 
        .HasForeignKey(m => m.VagaId)
        .OnDelete(DeleteBehavior.Cascade);  
    
    modelBuilder.Entity<Funcionario>()
        .ToTable("FUNCIONARIO");
    
    modelBuilder.Entity<Gerente>()
        .ToTable("GERENTE");
    
    modelBuilder.Entity<Gerente>()
        .HasIndex(g => g.CpfGerente)
        .IsUnique();
}

}