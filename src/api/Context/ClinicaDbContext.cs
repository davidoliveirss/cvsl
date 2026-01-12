using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Context;

public class ClinicaDbContext : DbContext
{
    public DbSet<Clinica> Clinicas { get; set; }
    public DbSet<Funcionario> Funcionarios { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Animal> Animais { get; set; }
    public DbSet<Relatorio> Relatorios { get; set; }


    private readonly IConfiguration _config;

    public ClinicaDbContext(DbContextOptions<ClinicaDbContext> options, IConfiguration config) : base(options)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _config.GetSection("DB")["ConnectionString"];
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações adicionais se necessário
        modelBuilder.Entity<Clinica>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<Funcionario>()
            .HasIndex(f => f.Email)
            .IsUnique();

        modelBuilder.Entity<Admin>()
            .HasIndex(a => a.Email)
            .IsUnique();
    }
}