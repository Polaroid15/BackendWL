using WL.Host.Entities;

namespace WL.Host.Services;

public interface IWishRepository
{
    Task AddWishAsync(Wish wish);
    Task<IEnumerable<Wish>> GetWishesAsync();
    Task<Wish?> GetWishAsync(Guid wishId);
    void Delete(Guid wishId);
    Task<bool> SaveChangesAsync();
}