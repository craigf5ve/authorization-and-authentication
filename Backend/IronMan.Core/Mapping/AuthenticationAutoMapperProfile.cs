using AutoMapper;
using IronMan.Core.Dtos.Authentication;
using IronMan.Data.Entities;

namespace IronMan.Core.Mapping
{

    public class AuthenticationAutoMapperProfile : Profile
    {
        public AuthenticationAutoMapperProfile()
        {
            CreateMap<Account, AccountResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role));

            CreateMap<Account, AuthenticateResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role));

            CreateMap<RegisterRequestDto, Account>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.ActivationToken, opt => opt.Ignore())
                .ForMember(dest => dest.Activated, opt => opt.Ignore())
                .ForMember(dest => dest.VerificationToken, opt => opt.Ignore())
                .ForMember(dest => dest.Verified, opt => opt.Ignore())
                .ForMember(dest => dest.ResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.ResetTokenExpires, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordReset, opt => opt.Ignore())
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());

            CreateMap<CreateRequestDto, Account>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.ActivationToken, opt => opt.Ignore())
                .ForMember(dest => dest.Activated, opt => opt.Ignore())
                .ForMember(dest => dest.VerificationToken, opt => opt.Ignore())
                .ForMember(dest => dest.Verified, opt => opt.Ignore())
                .ForMember(dest => dest.ResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.ResetTokenExpires, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordReset, opt => opt.Ignore())
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());

            CreateMap<UpdateRequestDto, Account>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        // ignore null role
                        if (x.DestinationMember.Name == "Role" && src.Role == null) return false;

                        return true;
                    }
                ));

            
        }
    }
}
