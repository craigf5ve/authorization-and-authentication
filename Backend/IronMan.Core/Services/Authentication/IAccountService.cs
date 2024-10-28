using IronMan.Core.Dtos;
using IronMan.Core.Dtos.Authentication;
using IronMan.Data.Entities;

namespace IronMan.Core.Services.Authentication
{
    public interface IAccountService
    {
        Task<ServiceResponse<AuthenticateResponseDto>> Authenticate(AuthenticateRequestDto model, string ipAddress);
        Task<ServiceResponse<AuthenticateResponseDto>> RefreshToken(string token, string ipAddress);
        Task<ServiceResponse<bool>> RevokeToken(string token, string ipAddress);
        Task<ServiceResponse<bool>> Register(RegisterRequestDto model, string? origin);
        Task<ServiceResponse<bool>> VerifyEmail(string token, string? origin);
        Task<ServiceResponse<bool>> ActivateAccount(string token, string? origin);
        Task<ServiceResponse<bool>> ForgotPassword(ForgotPasswordRequestDto model, string origin);
        Task<ServiceResponse<bool>> ValidateResetToken(ValidateResetTokenRequestDto model);
        Task<ServiceResponse<bool>> ResetPassword(ResetPasswordRequestDto model);
        Task<ServiceResponse<IEnumerable<AccountResponseDto>>> GetAll(Account account);
        Task<ServiceResponse<AccountResponseDto>> GetById(int id);
        Task<ServiceResponse<AccountResponseDto>> Create(CreateRequestDto model,  string origin, Account adminAccount);
        Task<ServiceResponse<AccountResponseDto>> Update(int id, UpdateRequestDto model, Account adminAccount);
        Task<ServiceResponse<bool>> Delete(int id, Account adminAccount);
        Task<ServiceResponse<bool>> ValidateToken(ValidateResetTokenRequestDto model);
        Task<ServiceResponse<bool>> ActivateAccountByAdmin(ActivateAccountByAdminRequestDto model, Account account, string? origin);
    }
}
