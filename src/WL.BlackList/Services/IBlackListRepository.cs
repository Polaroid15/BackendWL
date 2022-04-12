using Google.Protobuf.Collections;

namespace WL.BlackList.Services;

public interface IBlackListRepository<T>
{
    Task AddToBlackListAsync(T t);
    Task<IEnumerable<T>> GetItemsAsync();
    Task<T?> GetItemAsync(long id);
    void Delete(long id);
    Task<bool> SaveChangesAsync();
}