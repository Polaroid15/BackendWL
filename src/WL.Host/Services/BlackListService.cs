using WL.BlackList;
using BlackListItem = WL.Host.Entities.BlackListItem;

namespace WL.Host.Services;

public class BlackListService : IBlackListService
{
    private readonly BlackList.BlackListService.BlackListServiceClient _blackListService;

    public BlackListService(BlackList.BlackListService.BlackListServiceClient blackListService)
    {
        _blackListService = blackListService;
    }
    
    public async Task<List<BlackListItem>> GetBlackListItems(Guid wishId)
    {
        try
        {
            GetBlackListByWishIdRequest request = new GetBlackListByWishIdRequest { WishId = wishId.ToString() };

            BlackListResponse response = await _blackListService.GetBlackListAsync(request);

            var items = new List<BlackListItem>();
            foreach (var item in response.Items)
            {
                var blackListItem = new BlackListItem()
                {
                    Id = item.Id,
                    Name = item.Name,
                };
                items.Add(blackListItem);
            }
            
            return items;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
            
    }
    
}