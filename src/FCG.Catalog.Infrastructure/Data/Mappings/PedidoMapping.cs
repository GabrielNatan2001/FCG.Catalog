using FCG.Catalog.Domain.Pedidos.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Catalog.Infrastructure.Data.Mappings;

public class PedidoMapping : IEntityTypeConfiguration<PedidoEntity>
{
    public void Configure(EntityTypeBuilder<PedidoEntity> builder)
    {
        builder.ToTable("pedidos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("uuid");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.GameId)
            .HasColumnName("game_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnName("price")
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(x => x.DtCadastro)
            .HasColumnName("dt_cadastro")
            .IsRequired();

        builder.Property(x => x.DtAtualizacao)
            .HasColumnName("dt_atualizacao");
    }
}
