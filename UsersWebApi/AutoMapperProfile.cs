namespace UsersWebApi;

using AutoMapper;
using UsersWebApi.Entities;
using UsersWebApi.Models.Users;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    { 
        CreateMap<CreateUserRequest, User>();
         
        CreateMap<UpdateRequest, User>();

        CreateMap<User, UserViewModel>();
    }
}