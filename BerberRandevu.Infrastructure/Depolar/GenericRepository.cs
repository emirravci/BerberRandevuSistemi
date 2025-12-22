using System.Linq.Expressions;
using BerberRandevu.Application.Arayuzler.Depolar;
using BerberRandevu.Domain.Ortak;
using BerberRandevu.Infrastructure.VeriErisim;
using Microsoft.EntityFrameworkCore;

namespace BerberRandevu.Infrastructure.Depolar;

/// <summary>
/// EF Core tabanlı generic repository implementasyonu.
/// </summary>
public class GenericRepository<T> : IGenericRepository<T> where T : TemelVarlik
{
    protected readonly BerberDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(BerberDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    public virtual async Task<T?> GetirAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IReadOnlyList<T>> TumunuGetirAsync()
    {
        return await _dbSet.Where(x => !x.SilindiMi).ToListAsync();
    }

    public virtual async Task<IReadOnlyList<T>> FiltreliGetirAsync(Expression<Func<T, bool>> filtre)
    {
        return await _dbSet.Where(filtre).Where(x => !x.SilindiMi).ToListAsync();
    }

    public virtual async Task EkleAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual void Guncelle(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Sil(T entity)
    {
        entity.SilindiMi = true;
        _dbSet.Update(entity);
    }
}


