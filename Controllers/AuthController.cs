using HomeBankingAcc.DTOs;
using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeBankingAcc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public AuthController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                Client user = _clientRepository.FindByEmail(loginDTO.Email);
                if(user == null)
                {
                    return StatusCode(403, "User not found");
                }
                if (!user.Password.Equals(loginDTO.Password))
                {
                    return StatusCode(403, "Invalid password");
                }
                var claims = new List<Claim>
                {
                    new Claim("Client", user.Email)
                };
                if (user.Email.Equals("kaladin_bridge4@gmail.com"))
                {
                    Claim adminClaim = new Claim("Admin", user.Email);
                    claims.Add(adminClaim);
                }
                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );
                return Ok($"Welcome {loginDTO.Email}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok("Logged out succesfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
