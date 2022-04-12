using WL.Host.Entities;

namespace WL.Host.Services;

public interface IBlackListService
{
    Task<List<BlackListItem>> GetBlackListItems(Guid wishId);
}