using FCG.Catalog.Domain.Jogo.Entities;
using FCG.Catalog.Domain.Jogo.Interfaces;
using FCG.Catalog.Infrastructure.Data;

namespace FCG.Catalog.Infrastructure.Data.Repositories;

public class JogoRepository : Repository<JogoEntity>, IJogoRepository
{
    public JogoRepository(AppDbContext context) : base(context) { }
}
