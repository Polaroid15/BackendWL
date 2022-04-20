namespace WL.BlackList.Services;

public class BlackListItemRepository : IBlackListRepository<Entities.BlackListItem>
{
    public Task AddToBlackListAsync(Entities.BlackListItem t)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Entities.BlackListItem>> GetItemsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Entities.BlackListItem?> GetItemAsync(long id)
    {
        throw new NotImplementedException();
    }

    public void Delete(long id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}