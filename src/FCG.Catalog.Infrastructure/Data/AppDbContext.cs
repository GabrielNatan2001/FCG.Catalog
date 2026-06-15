using FCG.Catalog.Domain.Base;
using FCG.Catalog.Domain.Biblioteca.Entities;
using FCG.Catalog.Domain.Jogo.Entities;
using FCG.Catalog.Domain.Pedidos.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Catalog.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<JogoEntity> Jogos => Set<JogoEntity>();
    public DbSet<BibliotecaEntity> Bibliotecas => Set<BibliotecaEntity>();
    public DbSet<ItemBibliotecaEntity> ItensBiblioteca => Set<ItemBibliotecaEntity>();
    public DbSet<PedidoEntity> Pedidos => Set<PedidoEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            modelBuilder.Entity(entityType.ClrType)
                .Property(nameof(BaseEntity.Id))
                .ValueGeneratedNever();
        }

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.AtualizarDataAtualizacao();
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
