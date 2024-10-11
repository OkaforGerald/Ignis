using AutoMapper;
using Entities.Models;
using SharedAPI;
using Microsoft.AspNetCore.Identity.Data;

namespace Ignis
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<SharedAPI.DataTransfer.RegisterRequest, User>()
                .ForMember(x => x.UserName, options => options.MapFrom(x => x.Email));
        }
    }
}
