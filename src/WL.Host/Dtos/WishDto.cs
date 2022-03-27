namespace WL.Host.Dtos;

public record WishDto(Guid Id, int WishCategoryId, string Name, string Description);
public record UpdateWishDto(int WishCategoryId, string Name, string Description);