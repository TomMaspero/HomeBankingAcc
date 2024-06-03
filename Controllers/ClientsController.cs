using HomeBankingAcc.DTOs;
using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingAcc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public IActionResult GetAllClients()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();
                var clientsDTO = clients.Select(c => new ClientDTO(c)).ToList();
                return Ok(clientsDTO);
            } catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetClientById(long id) 
        {
            try
            {
                var client = _clientRepository.FindById(id);
                var clientDTO = new ClientDTO(client);
                return Ok(clientDTO);
            } 
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if(email == string.Empty)
                {
                    return Forbid();
                }
                Client client = _clientRepository.FindByEmail(email);
                if(client == null)
                {
                    return Forbid();
                }
                var clientDTO = new ClientDTO(client);
                return Ok(clientDTO);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Client client)
        {
            try
            {
                if(String.IsNullOrEmpty(client.Email) || 
                        String.IsNullOrEmpty(client.Password) ||
                        String.IsNullOrEmpty(client.FirstName) ||
                        String.IsNullOrEmpty(client.LastName)
                    ) {
                    return StatusCode(403, "datos inválidos");
                }
                Client user = _clientRepository.FindByEmail(client.Email);
                if(user != null)
                {
                    return StatusCode(403, "Email está en uso");
                }
                Client newClient = new Client
                {
                    Email = client.Email,
                    Password = client.Password,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                };
                _clientRepository.Save(newClient);
                return Created("", newClient);
            }
            catch(Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
