using FCG.Catalog.Domain.Biblioteca.Entities;
using FCG.Catalog.Domain.Jogo.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Catalog.Infrastructure.Data.Mappings;

public class BibliotecaMapping : IEntityTypeConfiguration<BibliotecaEntity>
{
    public void Configure(EntityTypeBuilder<BibliotecaEntity> builder)
    {
        builder.ToTable("bibliotecas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("uuid");

        builder.Property(x => x.UsuarioId)
            .HasColumnName("usuario_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasIndex(x => x.UsuarioId).IsUnique();

        builder.HasMany(x => x.Itens)
            .WithOne()
            .HasForeignKey(x => x.BibliotecaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Itens)
            .HasField("_itens")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(x => x.DtCadastro)
            .HasColumnName("dt_cadastro")
            .IsRequired();

        builder.Property(x => x.DtAtualizacao)
            .HasColumnName("dt_atualizacao");
    }
}

public class ItemBibliotecaMapping : IEntityTypeConfiguration<ItemBibliotecaEntity>
{
    public void Configure(EntityTypeBuilder<ItemBibliotecaEntity> builder)
    {
        builder.ToTable("itens_biblioteca");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("uuid");

        builder.Property(x => x.JogoId)
            .HasColumnName("jogo_id")
            .IsRequired();

        builder.Property(x => x.DataAquisicao)
            .HasColumnName("dt_aquisicao")
            .IsRequired();

        builder.HasOne(x => x.Jogo)
            .WithMany()
            .HasForeignKey(x => x.JogoId);

        builder.HasIndex(x => new { x.BibliotecaId, x.JogoId }).IsUnique();

        builder.Property(x => x.DtCadastro)
            .HasColumnName("dt_cadastro")
            .IsRequired();

        builder.Property(x => x.DtAtualizacao)
            .HasColumnName("dt_atualizacao");
    }
}
