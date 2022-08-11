using BootCamp.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BootCamp.WebAPI.Controllers
{
    [Route("v1/api")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]

        public ActionResult<dynamic> Authenticate([FromBody] Model.User loginParams) 
        {
            var user = Dal.UserDal.Get(loginParams.UserName, loginParams.Password);
            
            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(user);
            user.Password = "";
            return new
            {
                user = user,
                token = token
            };
        }
    }
}
