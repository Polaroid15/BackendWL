using Microsoft.EntityFrameworkCore;
using WL.Host.DbContexts;
using WL.Host.Entities;

namespace WL.Host.Services;

public class WishRepository : IWishRepository
{
    private readonly WishesContext _context;

    public WishRepository(WishesContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task AddWishAsync(Wish wish)
    {
        await _context.Wishes.AddAsync(wish);
    }

    public async Task<IEnumerable<Wish>> GetWishesAsync()
    {
        return await _context.Wishes.ToListAsync();
    }

    public async Task<Wish?> GetWishAsync(Guid wishId)
    {
        return await _context.Wishes.FirstOrDefaultAsync(x => x.Id == wishId);
    }

    public void Delete(Guid wishId)
    {
        var wish = new Wish() { Id = wishId };
        _context.Wishes.Remove(wish);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }
}