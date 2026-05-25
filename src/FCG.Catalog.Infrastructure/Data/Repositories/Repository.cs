using FCG.Catalog.Domain.Base;
using FCG.Catalog.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FCG.Catalog.Infrastructure.Data.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> ObterPorId(Guid id) => await DbSet.FindAsync(id);

    public async Task<IEnumerable<T>> ObterTodos() => await DbSet.ToListAsync();

    public async Task Adicionar(T entity) => await DbSet.AddAsync(entity);

    public void Atualizar(T entity) => DbSet.Update(entity);

    public void Remover(T entity) => DbSet.Remove(entity);

    public async Task<int> SalvarAlteracoes() => await Context.SaveChangesAsync();
}
