namespace BerberRandevu.Domain.Ortak;

/// <summary>
/// Tüm domain varlıkları için temel alanları içeren soyut sınıf.
/// </summary>
public abstract class TemelVarlik
{
    public int Id { get; set; }

    /// <summary>
    /// Varlığın oluşturulduğu tarih.
    /// </summary>
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Varlık güncellendiğinde set edilir.
    /// </summary>
    public DateTime? GuncellemeTarihi { get; set; }

    /// <summary>
    /// Soft delete için işaret alanı.
    /// </summary>
    public bool SilindiMi { get; set; }
}


