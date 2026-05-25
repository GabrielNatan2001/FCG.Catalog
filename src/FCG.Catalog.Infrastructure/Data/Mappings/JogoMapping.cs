using FCG.Catalog.Domain.Jogo.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Catalog.Infrastructure.Data.Mappings;

public class JogoMapping : IEntityTypeConfiguration<JogoEntity>
{
    public void Configure(EntityTypeBuilder<JogoEntity> builder)
    {
        builder.ToTable("jogos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("uuid");

        builder.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasColumnType("varchar(120)")
            .IsRequired();

        builder.Property(x => x.Descricao)
            .HasColumnName("descricao")
            .HasColumnType("varchar(1000)")
            .IsRequired();

        builder.Property(x => x.Preco)
            .HasColumnName("preco")
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        builder.Property(x => x.Categoria)
            .HasColumnName("categoria")
            .HasColumnType("varchar(100)")
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
