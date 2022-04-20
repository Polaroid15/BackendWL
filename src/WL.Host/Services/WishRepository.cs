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

    public async Task<(IEnumerable<Wish>, PaginationMetadata)> GetWishesAsync(
        string? name,
        string? searchQuery,
        int pageNumber,
        int pageSize)
    {
        var collection = _context.Wishes as IQueryable<Wish>;

        if (!string.IsNullOrEmpty(name))
        {
            name = name.Trim();
            collection = collection.Where(x => x.Name == name);
        }

        if (!string.IsNullOrEmpty(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection.Where(x => x.Name.Contains(searchQuery)
                                               || (x.Description != null && x.Description.Contains(searchQuery)));
        }

        var totalItemCount = await collection.CountAsync();

        var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);
        
        var resultCollection = await collection
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (resultCollection, paginationMetadata);
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