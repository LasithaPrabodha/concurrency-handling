namespace UsersWebApi;

using AutoMapper;
using UsersWebApi.Entities;
using UsersWebApi.Helpers;
using UsersWebApi.Models.Users;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    { 
        CreateMap<CreateUserRequestViewModel, User>();

        CreateMap<UpdateRequestViewModel, User>();
            //.ForMember(dest => dest.RowVersion, opt => opt.MapFrom(new CustomResolver()));

        CreateMap<User, UserResponseViewModel>();
    }
}

//public class CustomResolver : IValueResolver<UpdateRequestViewModel, User, byte[]>
//{
//    public byte[] Resolve(UpdateRequestViewModel source, User destination, byte[] member, ResolutionContext context)
//    {

//        if (Convert.ToInt64(ByteArrayToHexString(destination.RowVersion), 16) > Convert.ToInt64(ByteArrayToHexString((byte[])context.Items["etag"]), 16))
//            throw new PreconditionFailedException();

//        return destination.RowVersion;
//    }

//    private string ByteArrayToHexString(byte[] b)
//    {
//        System.Text.StringBuilder sb = new System.Text.StringBuilder();

//        sb.Append("0x");

//        foreach (byte val in b)
//        {
//            sb.Append(val.ToString("X2"));
//        }

//        return sb.ToString();
//    }
//}