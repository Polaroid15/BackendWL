namespace WL.Host.Dtos;

public record struct WishDto(Guid Id, int WishCategoryId, string Name, string Description);