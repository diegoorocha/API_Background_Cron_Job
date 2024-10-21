using API_Background.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Background.Data
{
    public class BackgroundDbContext(DbContextOptions<BackgroundDbContext> options) : DbContext(options)
    {
        public DbSet<Carro> Carro { get; set; }
        public DbSet<Venda> Venda { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<LogVendaProcessado> LogVendaProcessado { get; set; }
        public DbSet<CarroVenda> CarroVenda { get; set; } // Tabela intermediária para relacionamento muitos-para-muitos entre Venda e Carro

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Venda -> Cliente (Relacionamento de um para muitos)
            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Vendas)
                .HasForeignKey(v => v.ClienteId);

            // Venda -> CarroVenda (Relacionamento de muitos para muitos)
            modelBuilder.Entity<CarroVenda>()
                .HasKey(cv => new { cv.CarroId, cv.VendaId }); // Chave composta

            modelBuilder.Entity<CarroVenda>()
                .HasOne(cv => cv.Carro)
                .WithMany(c => c.CarrosVenda)
                .HasForeignKey(cv => cv.CarroId);

            modelBuilder.Entity<CarroVenda>()
                .HasOne(cv => cv.Venda)
                .WithMany(v => v.CarrosVenda)
                .HasForeignKey(cv => cv.VendaId);

            // Venda -> LogVendaProcessado (Relacionamento de um para muitos)
            modelBuilder.Entity<LogVendaProcessado>()
                .HasOne(l => l.Venda)
                .WithMany(v => v.LogsProcessamento)
                .HasForeignKey(l => l.VendaId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
