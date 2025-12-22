using System.Linq.Expressions;
using BerberRandevu.Domain.Ortak;

namespace BerberRandevu.Application.Arayuzler.Depolar;

/// <summary>
/// Tüm varlıklar için temel CRUD işlemlerini tanımlar.
/// </summary>
public interface IGenericRepository<T> where T : TemelVarlik
{
    Task<T?> GetirAsync(int id);

    Task<IReadOnlyList<T>> TumunuGetirAsync();

    Task<IReadOnlyList<T>> FiltreliGetirAsync(Expression<Func<T, bool>> filtre);

    Task EkleAsync(T entity);

    void Guncelle(T entity);

    void Sil(T entity);
}


