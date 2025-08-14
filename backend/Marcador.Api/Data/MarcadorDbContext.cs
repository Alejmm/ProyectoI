using Microsoft.EntityFrameworkCore;
using Marcador.Api.Models;

namespace Marcador.Api.Models
{
    public class MarcadorDbContext : DbContext
    {
        public MarcadorDbContext(DbContextOptions<MarcadorDbContext> options)
            : base(options) { }

        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<MarcadorGlobal> Marcadores { get; set; }
        public DbSet<Falta> Faltas { get; set; }

         // Aquí configuramos las relaciones para evitar múltiples paths de cascada
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MarcadorGlobal>()
                .HasOne(m => m.EquipoLocal)
                .WithMany()
                .HasForeignKey("EquipoLocalId")
                .OnDelete(DeleteBehavior.Restrict); // evita cascada

            modelBuilder.Entity<MarcadorGlobal>()
                .HasOne(m => m.EquipoVisitante)
                .WithMany()
                .HasForeignKey("EquipoVisitanteId")
                .OnDelete(DeleteBehavior.Restrict); // evita cascada
        }
     }
}
