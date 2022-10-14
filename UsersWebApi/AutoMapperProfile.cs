namespace UsersWebApi;

using AutoMapper;
using UsersWebApi.Entities;
using UsersWebApi.Models.Users;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    { 
        CreateMap<CreateUserRequestViewModel, User>();
         
        CreateMap<UpdateRequestViewModel, User>();

        CreateMap<User, UserReponseViewModel>();
    }
}