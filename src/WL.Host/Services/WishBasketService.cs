namespace WL.Host.Services;

public class WishBasketService : IWishBasketService
{
    private readonly HttpClient _httpClient;

    public WishBasketService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
}