using FCG.Catalog.Domain.Pedidos.Entities;
using FCG.Catalog.Domain.Pedidos.Interfaces;
using FCG.Catalog.Infrastructure.Data;

namespace FCG.Catalog.Infrastructure.Data.Repositories;

public class PedidoRepository : Repository<PedidoEntity>, IPedidoRepository
{
    public PedidoRepository(AppDbContext context) : base(context) { }
}
