using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Venta.API.Models;
using Venta.API.Security;

namespace Venta.API.Controllers
{
    namespace Venta.Api.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class AuthController : Controller
        {
            private readonly IConfiguration _Configuration;
            private readonly string _secret;
            private readonly string _expire_hours;

            public AuthController(IMediator mediator, IConfiguration configuration)
            {
                _Configuration = configuration;
                _secret = configuration["KEY-JWT"];
                _expire_hours = configuration["EXPIRE-HOURS"];
            }

            [HttpPost]
            [AllowAnonymous]
            public async Task<IActionResult> Login(LoginModel model)
            {
                var token = TokenServices.CreateToken(model,_secret,Convert.ToDouble(_expire_hours));

                return Ok(token);
            }
        }
    }
}
