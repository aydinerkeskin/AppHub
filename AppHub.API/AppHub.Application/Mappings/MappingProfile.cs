using AutoMapper;
using AppHub.Domain.Entities;
using AppHub.Application.DTOs.OAuthProvider;
using AppHub.Application.DTOs.User;
using AppHub.Application.DTOs.UserOAuth;
using AppHub.Application.DTOs.Application;
using ApplicationEntity = AppHub.Domain.Entities.Application;

namespace AppHub.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // OAuthProvider mappings
        CreateMap<OAuthProvider, OAuthProviderDto>().ReverseMap();
        CreateMap<CreateOAuthProviderDto, OAuthProvider>();
        CreateMap<UpdateOAuthProviderDto, OAuthProvider>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // User mappings
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // UserOAuth mappings
        CreateMap<UserOAuth, UserOAuthDto>().ReverseMap();
        CreateMap<CreateUserOAuthDto, UserOAuth>();
        CreateMap<UpdateUserOAuthDto, UserOAuth>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Application mappings
        CreateMap<ApplicationEntity, ApplicationDto>().ReverseMap();
        CreateMap<CreateApplicationDto, ApplicationEntity>();
        CreateMap<UpdateApplicationDto, ApplicationEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}

