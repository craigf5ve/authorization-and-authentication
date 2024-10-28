using IronMan.Data.DbContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace IronMan.Core.Helpers.Authentication
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IronManDbContext dataContext, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var UserId = jwtUtils.ValidateJwtToken(token);
            if (UserId != null)
            {
                // attach User to context on successful jwt validation
                context.Items["Account"] = await dataContext.Accounts
                .FirstOrDefaultAsync(x => x.Id == UserId && !x.IsDeleted && x.Verified.HasValue && x.Activated.HasValue);
            }

            await _next(context);
        }
    }

}
