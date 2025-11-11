using AutoMapper;
using Backend.Data.Models;
using Backend.Models;

namespace Backend.Data;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserCredentials>();
        CreateMap<User, UserInfo>();

        CreateMap<Models.Task, TaskInfo>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
    }

    public static TEnumTarget MapEnum<TEnumSource, TEnumTarget>(TEnumSource sourceEnum)
        where TEnumSource : Enum
        where TEnumTarget : Enum
    {
        if (Enum.IsDefined(typeof(TEnumTarget), Convert.ToInt32(sourceEnum)))
        {
            return (TEnumTarget)Enum.ToObject(typeof(TEnumTarget), sourceEnum);
        }
        throw new ArgumentOutOfRangeException(nameof(sourceEnum), "Unknown enum value");
    }
}
