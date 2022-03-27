using AutoMapper;
using WL.Host.Dtos;
using WL.Host.Entities;

namespace WL.Host;

public class WishProfile : Profile
{
    public WishProfile()
    {
        CreateMap<Wish, WishDto>();
        CreateMap<UpdateWishDto, Wish>();
        CreateMap<Wish, UpdateWishDto>();
    }
}