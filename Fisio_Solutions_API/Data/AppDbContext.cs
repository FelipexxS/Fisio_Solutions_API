using Microsoft.EntityFrameworkCore;
using Fisio_Solutions_API.Models;

namespace Fisio_Solutions_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Exercicio> Exercicios => Set<Exercicio>();
        public DbSet<ListaExercicios> ListasExercicios => Set<ListaExercicios>();
        public DbSet<ListaExercicioItem> ListaExercicioItens => Set<ListaExercicioItem>();
        public DbSet<DicaErgonomia> DicasErgonomia => Set<DicaErgonomia>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(200);
                entity.Property(u => u.Profession).HasMaxLength(150);
                entity.Property(u => u.BirthDate).HasMaxLength(20);
                entity.Property(u => u.PasswordHash).IsRequired();
            });

            modelBuilder.Entity<Exercicio>(entity =>
            {
                entity.ToTable("Exercicios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(e => e.RegiaoCorpo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Objetivo).HasMaxLength(50);
            });

            modelBuilder.Entity<ListaExercicios>(entity =>
            {
                entity.ToTable("Listas_Exercicios");
                entity.HasKey(l => l.Id);
                entity.Property(l => l.NomeLista).IsRequired().HasMaxLength(150);
                entity.Property(l => l.Categoria).HasMaxLength(50);

                // Relacionamento com Usuário (um usuário pode ter apenas uma lista geral ou lista por usuário)
                entity.HasOne(l => l.Usuario)
                      .WithMany()
                      .HasForeignKey(l => l.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relacionamento com Lista_Exercicio_Itens
                entity.HasMany(l => l.Itens)
                      .WithOne(i => i.Lista)
                      .HasForeignKey(i => i.ListaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ListaExercicioItem>(entity =>
            {
                entity.ToTable("Lista_Exercicio_Itens");
                entity.HasKey(i => i.Id);

                // Relacionamento com Exercício
                entity.HasOne(i => i.Exercicio)
                      .WithMany(e => e.ListaItens)
                      .HasForeignKey(i => i.ExercicioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DicaErgonomia>(entity =>
            {
                entity.ToTable("Dicas_Ergonomia");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Titulo).IsRequired().HasMaxLength(250);
                entity.Property(d => d.Conteudo).IsRequired();
                entity.Property(d => d.ImagemIlustracaoUrl).HasMaxLength(300);
                entity.Property(d => d.Ativo).HasDefaultValue(true);
                entity.Property(d => d.CreatedAt).IsRequired();
            });
        }
    }
}

