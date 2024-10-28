using IronMan.Api.Controllers.Shared;
using IronMan.Core.Dtos;
using IronMan.Core.Dtos.Authentication;
using IronMan.Core.Helpers.Authentication;
using IronMan.Core.Services.Authentication;
using IronMan.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IronMan.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class AccountsController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<ServiceResponse<AuthenticateResponseDto>>> Authenticate(AuthenticateRequestDto model)
        {
            var response = await _accountService.Authenticate(model, IpAddress());
            SetTokenCookie(response!.Data!.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ServiceResponse<AuthenticateResponseDto>>> RefreshToken()
        
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _accountService.RefreshToken(refreshToken!, IpAddress());
            SetTokenCookie(response.Data!.RefreshToken);
            return Ok(response);
        }

       
        [HttpPost("revoke-token")]
        public async Task<ActionResult<ServiceResponse<bool>>> RevokeToken(RevokeTokenRequestDto model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            // users can revoke their own tokens and admins can revoke any tokens
            if (!Account.OwnsToken(token) && Account.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            return Ok(await _accountService.RevokeToken(token, IpAddress()));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<bool>>> Register(RegisterRequestDto model)
        {
            return Ok(await _accountService.Register(model, Request.Headers.Origin));
        }

        [AllowAnonymous]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailRequestDto model)
        {
            return Ok(await _accountService.VerifyEmail(model.Token, Request.Headers.Origin));
        }

        [AllowAnonymous]
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmailDirect([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required.");
            }
            return Ok(await _accountService.VerifyEmail(token, Request.Headers.Origin));
        }
        
        [AllowAnonymous]
        [HttpPost("activate-account")]
        public async Task<ActionResult<ServiceResponse<bool>>> ActivateAccount(VerifyEmailRequestDto model)
        {
            return Ok(await _accountService.ActivateAccount(model.Token, Request.Headers.Origin));
        }

        [AllowAnonymous]
        [HttpGet("activate-account")]
        public async Task<ActionResult<ServiceResponse<bool>>> ActivateAccountDirect([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required.");
            }
            return Ok(await _accountService.ActivateAccount(token, Request.Headers.Origin));
        }
        
        [Authorize(Role.Admin, Role.SuperAdmin)]
        [HttpPost("activate-account-by-admin")]
        public async Task<ActionResult<ServiceResponse<bool>>>  ActivateAccountByAdmin(ActivateAccountByAdminRequestDto model)
        {
            return Ok(await _accountService.ActivateAccountByAdmin(model, Account, Request.Headers.Origin));
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult<ServiceResponse<bool>>> ForgotPassword(ForgotPasswordRequestDto model)
        {
            ;
            return Ok(await _accountService.ForgotPassword(model, Request.Headers.Origin!));
        }

        [AllowAnonymous]
        [HttpPost("validate-reset-token")]
        public async Task<ActionResult<ServiceResponse<bool>>> ValidateResetToken(ValidateResetTokenRequestDto model)
        {
            return Ok(await _accountService.ValidateResetToken(model));
        }

        [AllowAnonymous]
        [HttpPost("validate-token")]
        public async Task<ActionResult<ServiceResponse<bool>>> ValidateToken(ValidateResetTokenRequestDto model)
        {
             return Ok(await _accountService.ValidateToken(model));
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<ActionResult<ServiceResponse<bool>>> ResetPassword(ResetPasswordRequestDto model)
        {
            return Ok(await _accountService.ResetPassword(model));
        }

        [Authorize(Role.Admin, Role.SuperAdmin)]
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<IEnumerable<AccountResponseDto>>>> GetAll()
        {
            return Ok(await _accountService.GetAll(Account));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ServiceResponse<AccountResponseDto>>> GetById(int id)
        {
            // users can get their own account and admins can get any account
            return id != Account.Id && Account.Role != Role.Admin
                ? throw new UnauthorizedAccessException("Unauthorized")
                : (ActionResult<ServiceResponse<AccountResponseDto>>)Ok(await _accountService.GetById(id));
        }

        [Authorize(Role.Admin, Role.SuperAdmin)]
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<ServiceResponse<AccountResponseDto>>>> Create(CreateRequestDto model)
        {
            return Ok(await _accountService.Create(model, Request.Headers.Origin!, Account));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ServiceResponse<AccountResponseDto>>> Update(int id, UpdateRequestDto model)
        {
            // users can update their own account and admins can update any account
            if (id != Account.Id && (Account.Role != Role.Admin && Account.Role != Role.SuperAdmin))
                throw new UnauthorizedAccessException("Unauthorized");

            // only admins can update role
            if (Account.Role != Role.Admin || Account.Role != Role.SuperAdmin)
                model.Role = null;

            return Ok(await _accountService.Update(id, model, Account));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ServiceResponse<bool>>> Delete(int id)
        {
            // users can delete their own account and admins can delete any account
            if (id != Account.Id && (Account.Role != Role.Admin && Account.Role != Role.SuperAdmin))
                throw new UnauthorizedAccessException("Unauthorized");
            return Ok(await _accountService.Delete(id, Account));
        }

        #region Helper Methods

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Expires = DateTime.UtcNow.AddDays(7),
                SameSite= SameSiteMode.None,
                Secure =true
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        #endregion
    }
}
