using FCG.Catalog.Domain.Base;
using FCG.Catalog.Domain.Common.Enums;
using FCG.Catalog.Domain.Exceptions;

namespace FCG.Catalog.Domain.Pedidos.Entities;

public class PedidoEntity : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public decimal Price { get; private set; }
    public EPedidoStatus Status { get; private set; }

    protected PedidoEntity() { }

    private PedidoEntity(Guid userId, Guid gameId, decimal price)
    {
        UserId = userId;
        GameId = gameId;
        Price = price;
        Status = EPedidoStatus.Pending;
    }

    public static PedidoEntity Criar(Guid userId, Guid gameId, decimal price)
    {
        if (price < 0)
            throw new DomainException("Preço do pedido não pode ser negativo.");

        return new PedidoEntity(userId, gameId, price);
    }

    public void Completar()
    {
        Status = EPedidoStatus.Completed;
        AtualizarDataAtualizacao();
    }

    public void Rejeitar()
    {
        Status = EPedidoStatus.Rejected;
        AtualizarDataAtualizacao();
    }
}
