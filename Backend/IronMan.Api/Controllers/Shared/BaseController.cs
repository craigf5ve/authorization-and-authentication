using IronMan.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IronMan.Api.Controllers.Shared
{
    [Controller]
    public abstract class BaseController : ControllerBase
    {
        public Account Account => (Account)HttpContext.Items["Account"]!;
    }
}
