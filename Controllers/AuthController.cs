using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeBankingAcc.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IClientRepository _clientRepository;
        public AuthController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Client client)
        {
            try
            {
                Client user = _clientRepository.FindByEmail(client.Email);
                if (user == null || !String.Equals(user.Password, client.Password)){
                    return Unauthorized();
                }
                var claims = new List<Claim>
                {
                    new Claim("Client", user.Email),
                };
                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                    );
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
