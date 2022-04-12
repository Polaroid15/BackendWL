using Google.Protobuf.Collections;
using Grpc.Core;

namespace WL.BlackList.Services;

public class BlackListService : BlackList.BlackListService.BlackListServiceBase
{
    private readonly IBlackListRepository<Entities.BlackListItem> _repository;
    private readonly ILogger<BlackListService> _logger;

    public BlackListService(IBlackListRepository<Entities.BlackListItem> repository, ILogger<BlackListService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override async Task<BlackListResponse> GetBlackList(GetBlackListByWishIdRequest request, ServerCallContext context)
    {
        var response = new BlackListResponse();
        // RepeatedField<Entities.BlackListItem> items = await _repository.GetItemsAsync();
        response.Items.Add(new BlackListItem() { Id = 1, Name = "testName" });
        return await Task.FromResult(response);
    }
}