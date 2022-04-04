using WL.Host.Entities;

namespace WL.Host.Dtos;

/// <summary>
/// A DTO for <see cref="Wish"/>
/// </summary>
/// <param name="Id">Identification number</param>
/// <param name="WishCategoryId">Category Id</param>
/// <param name="Name">Name</param>
/// <param name="Description">Description</param>
public record WishDto(Guid Id, int WishCategoryId, string Name, string Description);

/// <summary>
/// A DTO for updating wishes
/// </summary>
/// <param name="WishCategoryId">Category Id</param>
/// <param name="Name">Name</param>
/// <param name="Description">Description</param>
public record UpdateWishDto(int WishCategoryId, string Name, string Description);