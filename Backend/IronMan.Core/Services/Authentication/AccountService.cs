using AutoMapper;
using IronMan.Core.Dtos;
using IronMan.Core.Dtos.Authentication;
using IronMan.Core.Helpers.Authentication;
using IronMan.Core.Helpers;
using IronMan.Data.DbContexts;
using IronMan.Data.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Microsoft.Extensions.Logging;

namespace IronMan.Core.Services.Authentication
{

    public class AccountService : IAccountService
    {
        private readonly IronManDbContext _context;
        private readonly IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            IronManDbContext context,
            IJwtUtils jwtUtils,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IEmailService emailService,
            ILogger<AccountService> logger)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ServiceResponse<AuthenticateResponseDto>> Authenticate(AuthenticateRequestDto model, string ipAddress)
        {
            try
            {
                _logger.LogInformation("Authenticating user: {Email}", model.Email);

                var account = await _context.Accounts.SingleOrDefaultAsync(x => x.Email == model.Email);

                // validate
                if (account == null || account.IsDeleted || !account.IsVerified || !account.IsActivated || !BCrypt.Net.BCrypt.Verify(model.Password, account.PasswordHash))
                {
                    _logger.LogWarning("Authentication failed for user: {Email}", model.Email);
                    throw new AppException("Email or password is incorrect. Please contact your Administrator if you're having trouble logging in.");
                }

                // authentication successful so generate jwt and refresh tokens
                var jwtToken = _jwtUtils.GenerateJwtToken(account);
                var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
                account.RefreshTokens.Add(refreshToken);

                // remove old refresh tokens from account
                RemoveOldRefreshTokens(account);

                // save changes to db
                _context.Update(account);
                await _context.SaveChangesAsync();

                var response = _mapper.Map<AuthenticateResponseDto>(account);
                response.JwtToken = jwtToken;
                response.RefreshToken = refreshToken.Token;

                _logger.LogInformation("User authenticated successfully: {Email}", model.Email);

                return new ServiceResponse<AuthenticateResponseDto>(response, "Login Successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during authentication");
                throw new AppException("Authentication failed");
            }
        }

        public async Task<ServiceResponse<AuthenticateResponseDto>> RefreshToken(string token, string ipAddress)
        {
            try
            {
                var account = await GetAccountByRefreshToken(token);
                var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

                if (refreshToken.IsRevoked)
                {
                    // revoke all descendant tokens in case this token has been compromised
                    RevokeDescendantRefreshTokens(refreshToken, account, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }

                if (!refreshToken.IsActive)
                    throw new AppException("Invalid token");

                // replace old refresh token with a new one (rotate token)
                var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
                account.RefreshTokens.Add(newRefreshToken);

                // remove old refresh tokens from account
                RemoveOldRefreshTokens(account);

                // save changes to db
                _context.Update(account);
                await _context.SaveChangesAsync();

                // generate new jwt
                var jwtToken = _jwtUtils.GenerateJwtToken(account);

                // return data in authenticate response object
                var response = _mapper.Map<AuthenticateResponseDto>(account);
                response.JwtToken = jwtToken;
                response.RefreshToken = newRefreshToken.Token;
                return new ServiceResponse<AuthenticateResponseDto>(response, "Token Refreshed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                throw new AppException("Invalid token");
            }
        }

        public async Task<ServiceResponse<bool>> RevokeToken(string token, string ipAddress)
        {
            var account = await GetAccountByRefreshToken(token);
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
            {
                _logger.LogWarning("Invalid token");
                throw new AppException("Invalid token");
            }

            // revoke token and save
            RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            _context.Update(account);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Token revoked");

            return new ServiceResponse<bool>(true, "Token revoked");
        }

        public async Task<ServiceResponse<bool>> Register(RegisterRequestDto model, string? origin)
        {
            try
            {
                // validate
                if (await _context.Accounts.AnyAsync(x => x.Email == model.Email && !x.IsDeleted))
                {
                    // send already registered error in email to prevent account enumeration
                    await SendAlreadyRegisteredEmail(model.Email, origin);
                    throw new AppException("Email is already registered");
                }

                // map model to new account object
                var account = _mapper.Map<Account>(model);

                // first registered account is an admin
                var isFirstAccount = !await _context.Accounts.AnyAsync();
                account.Role = isFirstAccount ? Role.SuperAdmin : model.Role;

                account.Activated = isFirstAccount ? DateTime.UtcNow : null;
                account.ActivationToken = isFirstAccount ? null : await GenerateActivationToken();
                account.Created = DateTime.UtcNow;
                account.VerificationToken = await GenerateVerificationToken();

                // hash password
                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // save account
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();
                await SendVerificationEmail(account, origin);

                return new ServiceResponse<bool>(true, "Registration successful, please check your email for verification instructions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration");
                throw new AppException("Registration failed");
            }
        }

        public async Task<ServiceResponse<bool>> VerifyEmail(string token, string? origin)
        {
            try
            {
                var account = await _context.Accounts
                    .SingleOrDefaultAsync(x => x.VerificationToken == token) ?? throw new AppException("Verification failed");
                account.Verified = DateTime.Now;
                account.VerificationToken = null;

                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                // send email
                await SendVerificationConfirmationEmail(account, origin);

                if (account.Role == Role.Admin || account.Role == Role.SuperAdmin)
                {
                    var admins = await _context.Accounts.Where(x => x.Role == Role.SuperAdmin && !x.IsDeleted && x.Activated.HasValue && x.Id != account.Id).ToListAsync();
                    var adminEmails = admins.Select(x => x.Email).ToList();
                    await SendAdminActivationEmail(account, origin, adminEmails);
                }
                else if (account.Role == Role.Manager || account.Role == Role.Clerk)
                {
                    var locationAdmins = await _context.Accounts.Where(x => x.Role == Role.Admin && !x.IsDeleted && x.Activated.HasValue && x.Id != account.Id).ToListAsync();
                    var adminEmails = locationAdmins.Select(x => x.Email).ToList();
                    await SendAdminActivationEmail(account, origin, adminEmails);
                }

                _logger.LogInformation("Email verified");
                return new ServiceResponse<bool>(true, "Account Verified");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email");
                throw new AppException("Verification failed");
            }
        }

        public async Task<ServiceResponse<bool>> ActivateAccount(string token, string? origin)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(x => x.ActivationToken == token) ?? throw new AppException("Activation failed");
            if (account.IsActivated)
                throw new AppException("Account already activated");

            account.Activated = DateTime.UtcNow;
            account.ActivationToken = null;

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            await SendActivationConfirmationEmail(account, origin);

            return new ServiceResponse<bool>(true, "Account Activated");
        }

        public async Task<ServiceResponse<bool>> ActivateAccountByAdmin(ActivateAccountByAdminRequestDto model, Account adminAccount, string origin)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.Id == model.Id && !a.IsDeleted) ?? throw new AppException("Activation failed");
            if (account.IsActivated)
                throw new AppException("Account already activated");

            account.Activated = DateTime.UtcNow;
            account.ActivationToken = null;
            account.PrepareEntityForCreate(account);
            account.PrepareEntityForUpdate(adminAccount);
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            await SendActivationConfirmationEmail(account, origin);

            return new ServiceResponse<bool>(true, "Account Activated");
        }

        public async Task<ServiceResponse<bool>> ForgotPassword(ForgotPasswordRequestDto model, string origin)
        {
            try
            {
                var account = await _context.Accounts.SingleOrDefaultAsync(x => x.Email == model.Email && x.Activated.HasValue) ?? throw new AppException("No account found with this email");

                // create reset token that expires after 1 day
                account.ResetToken = await GenerateResetToken();
                account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                await SendPasswordResetEmail(account, origin);
                return new ServiceResponse<bool>(true, "Please check your email for password reset instructions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Forgot password failed");
                throw new AppException(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> ValidateResetToken(ValidateResetTokenRequestDto model)
        {
            await GetAccountByResetToken(model.Token);
            return new ServiceResponse<bool>(true, "Reset Token is valid");
        }

        public async Task<ServiceResponse<bool>> ResetPassword(ResetPasswordRequestDto model)
        {
            var account = await GetAccountByResetToken(model.Token);

            // update password and remove reset token
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            account.PasswordReset = DateTime.UtcNow;
            account.ResetToken = null;
            account.ResetTokenExpires = null;

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>(true, "Password Reset Successful");
        }

        public async Task<ServiceResponse<IEnumerable<AccountResponseDto>>> GetAll(Account account)
        {
            var query = _context.Accounts
                .Where(a => !a.IsDeleted);

            var accounts = await query.ToListAsync();
            return new ServiceResponse<IEnumerable<AccountResponseDto>>(_mapper.Map<IEnumerable<AccountResponseDto>>(accounts), "Accounts Successfully retreived");
        }

        public async Task<ServiceResponse<AccountResponseDto>> GetById(int id)
        {
            var account = await GetAccount(id);
            return new ServiceResponse<AccountResponseDto>(_mapper.Map<AccountResponseDto>(account), "Account Found");
        }

        public async Task<ServiceResponse<AccountResponseDto>> Create(CreateRequestDto model, string origin, Account adminAccount)
        {
            // validate
            if (await _context.Accounts.AnyAsync(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");
            // map model to new account object
            var account = _mapper.Map<Account>(model);
            account.PrepareForCreateAndUpdate(adminAccount);
            account.Created = DateTime.UtcNow;
            account.VerificationToken = await GenerateVerificationToken();
            account.Activated = DateTime.UtcNow;

            // hash password
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // save account
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            await SendVerificationEmail(account, origin);

            return new ServiceResponse<AccountResponseDto>(_mapper.Map<AccountResponseDto>(account), "Account Created");
        }

        public async Task<ServiceResponse<AccountResponseDto>> Update(int id, UpdateRequestDto model, Account adminAccount)
        {
            var account = await GetAccount(id);

            // validate
            if (account.Email != model.Email && _context.Accounts.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");

            // copy model to account and save
            _mapper.Map(model, account);
            account.PrepareEntityForUpdate(adminAccount);
            _context.Accounts.Update(account);
            _context.SaveChanges();

            return new ServiceResponse<AccountResponseDto>(_mapper.Map<AccountResponseDto>(account), "Account Updated");
        }

        public async Task<ServiceResponse<bool>> Delete(int id, Account adminAccount)
        {
            var account = await GetAccount(id);
            account.Verified = null;
            account.Activated = null;
            account.PrepareEntityForDelete(adminAccount);
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(true, "Account Deleted");
        }

        public async Task<ServiceResponse<bool>> ValidateToken(ValidateResetTokenRequestDto model)
        {
            var UserId = _jwtUtils.ValidateJwtToken(model.Token);
            return new ServiceResponse<bool>(UserId != null, "Token is valid");
        }


        #region Helper Methods

        private async Task<Account> GetAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            return account ?? throw new KeyNotFoundException("Account not found");
        }

        private async Task<Account> GetAccountByRefreshToken(string token)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token) && !u.IsDeleted && u.Verified.HasValue && u.Activated.HasValue);
            return account ?? throw new AppException("Invalid token");
        }

        private async Task<Account> GetAccountByResetToken(string token)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(x =>
                x.ResetToken == token && x.ResetTokenExpires > DateTime.UtcNow);
            return account ?? throw new AppException("Invalid token");
        }

        private string GenerateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<string> GenerateResetToken()
        {
            // token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // ensure token is unique by checking against db
            var tokenIsUnique = !await _context.Accounts.AnyAsync(x => x.ResetToken == token);
            if (!tokenIsUnique)
                return await GenerateResetToken();

            return token;
        }

        private async Task<string> GenerateVerificationToken()
        {
            // token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // ensure token is unique by checking against db
            var tokenIsUnique = !await _context.Accounts.AnyAsync(x => x.VerificationToken == token);
            if (!tokenIsUnique)
                return await GenerateVerificationToken();

            return token;
        }

        private async Task<string> GenerateActivationToken()
        {
            // token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // ensure token is unique by checking against db
            var tokenIsUnique = !await _context.Accounts.AnyAsync(x => x.ActivationToken == token);
            if (!tokenIsUnique)
                return await GenerateActivationToken();

            return token;
        }


        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private void RemoveOldRefreshTokens(Account account)
        {
            account.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private void RevokeDescendantRefreshTokens(RefreshToken refreshToken, Account account, string ipAddress, string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = account.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken!.IsActive)
                    RevokeRefreshToken(childToken, ipAddress, reason);
                else
                    RevokeDescendantRefreshTokens(childToken, account, ipAddress, reason);
            }
        }

        private void RevokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }

        private async Task SendVerificationEmail(Account account, string? origin)
        {
            string message;
            string verifyUrl;
            if (!string.IsNullOrEmpty(origin))
            {
                // origin exists if request sent from browser single page app (e.g. Angular or React)
                // so send link to verify via single page app
                verifyUrl = $"{origin}/#/auth/verify-email?token={account.VerificationToken}";
                message = $@"<p>Please click the below link to verify your email address:</p>";
            }
            else
            {
                // origin missing if request sent directly to api (e.g. from Postman)
                // so send instructions to verify directly with api
                message = $@"<p>Please use the below token to verify your email address with the <code>/auth/verify-email</code> api route:</p>
                            <p><code>{account.VerificationToken}</code></p>";
                verifyUrl = $"{origin}";
            }

            await _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Verify Email",
                body: new EmailBodyDto
                {
                    subject = $@"Verify Email",
                    body = $@"<p>Thanks for registering!</p>{message}",
                    buttontext = "Verify Email",
                    link = verifyUrl
                }
            );

        }

        private async Task SendAlreadyRegisteredEmail(string email, string? origin)
        {
            string message;
            string forgotpasswordUrl = "";
            if (!string.IsNullOrEmpty(origin))
            {
                message = $@"<p>If you don't know your password please visit the <a href=""{origin}/auth/forgot-password"">forgot password</a> page.</p>";
                forgotpasswordUrl = $@"{origin}/account/forgot-password";
            }
            else
            {
                message = "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

            }

            await _emailService.Send(
                to: email,
                subject: "Sign-up Verification API - Email Already Registered",
                 body: new EmailBodyDto
                 {
                     subject = $@"Email Already Registered",
                     body = $@"<p>Your email <strong>{email}</strong> is already registered.</p>
                        {message}",
                     buttontext = "Verify Email",
                     link = forgotpasswordUrl
                 }
            );
        }

        private async Task SendAdminActivationEmail(Account account, string? origin, List<string> adminEmails)
        {
            try
            {
                string message;
                string activateUrl;
                var accountDetails = $@"
                    <h4>Account Details</h4>
                    <p>Name: {account.Title} {account.FirstName} {account.LastName}</p>
                    <p>Role : {account.Role.ToString()}";
                if (!string.IsNullOrEmpty(origin))
                {
                    // origin exists if request sent from browser single page app (e.g. Angular or React)
                    // so send link to verify via single page app
                    activateUrl = $"{origin}/#/auth/activate-account?token={account.ActivationToken}";

                    message = $@"
                    {accountDetails}

                    <p>Please click the below link to activate the account</p>";
                }
                else
                {
                    // origin missing if request sent directly to api (e.g. from Postman)
                    // so send instructions to verify directly with api
                    message = $@"
                    {accountDetails}

                    c<p>Please use the below token to activate the account with the <code>/auth/activate-account</code> api route:</p>
                    <p><code>{account.ActivationToken}</code></p>";
                    activateUrl = "";
                }

                foreach (var email in adminEmails)
                {
                    await _emailService.Send(
                    to: email,
                    subject: "Shearwater NTS System - Activation Email",
                    body: new EmailBodyDto
                    {
                        subject = $@"Activate Account",
                        body = message,
                        buttontext = "Activate Account",
                        link = activateUrl
                    }
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending admin activation email");
                throw; // rethrow the exception to propagate it to the caller
            }
        }

        private async Task SendVerificationConfirmationEmail(Account account, string? origin)
        {
            try
            {
                var extra = account.IsActivated ? "You may now log in." : "You will be notified when your account is activated by an administrator.";
                var message = $@"<p>Your account has been verified successfully. {extra}</p>";

                await _emailService.Send(
                    to: account.Email,
                    subject: "Shearwater NTS System - Account Verified",
                    body: new EmailBodyDto
                    {
                        subject = "Account Verified",
                        body = $@"{message}",
                        buttontext = "",
                        link = "#"
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send verification confirmation email");
                throw; // rethrow the exception to be handled by the caller
            }
        }

        private async Task SendActivationConfirmationEmail(Account account, string origin)
        {
            try
            {
                await _emailService.Send(
                    to: account.Email,
                    subject: "Shearwater NTS System - Account Activated",
                    body: new EmailBodyDto
                    {
                        subject = "Account Activated",
                        body = "<p>Your account has been activated successfully. You may now log in.</p>",
                        buttontext = "Login Here",
                        link = $@"{origin}"
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send activation confirmation email");
                // Handle the exception or throw it to the caller
                throw;
            }
        }
        private async Task SendPasswordResetEmail(Account account, string? origin)
        {
            try
            {
                string message;
                string resetUrl;
                if (!string.IsNullOrEmpty(origin))
                {
                    resetUrl = $"{origin}/#/auth/reset-password?token={account.ResetToken}";
                    message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>";
                }
                else
                {
                    message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                                <p><code>{account.ResetToken}</code></p>";
                    resetUrl = "";
                }

                await _emailService.Send(
                    to: account.Email,
                    subject: "Sign-up Verification API - Reset Password",
                    body: new EmailBodyDto
                    {
                        subject = "Reset Password Email",
                        body = $@"{message}",
                        buttontext = "Reset Password",
                        link = resetUrl
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email");
                throw; // Rethrow the exception to propagate it to the caller
            }
        }
        #endregion
    }
}
