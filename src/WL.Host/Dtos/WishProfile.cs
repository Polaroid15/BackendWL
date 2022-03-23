using AutoMapper;
using WL.Host.Entities;

namespace WL.Host.Dtos;

public class WishProfile : Profile
{
    public WishProfile()
    {
        CreateMap<Wish, WishDto>();
    }
}