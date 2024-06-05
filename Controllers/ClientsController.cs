using HomeBankingAcc.DTOs;
using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if(email == string.Empty)
                {
                    return StatusCode(403, "User not found");
                }
                Client client = _clientRepository.FindByEmail(email);
                if(client == null)
                {
                    return StatusCode(403, "User not found");
                }
                return Ok(new ClientDTO(client));
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //[HttpPost]
        //public IActionResult Post([FromBody] Client client)
        //{
        //    try
        //    {
        //        if(String.IsNullOrEmpty(client.Email) || 
        //                String.IsNullOrEmpty(client.Password) ||
        //                String.IsNullOrEmpty(client.FirstName) ||
        //                String.IsNullOrEmpty(client.LastName)
        //            ) {
        //            return StatusCode(403, "datos inválidos");
        //        }
        //        Client user = _clientRepository.FindByEmail(client.Email);
        //        if(user != null)
        //        {
        //            return StatusCode(403, "Email está en uso");
        //        }
        //        Client newClient = new Client
        //        {
        //            Email = client.Email,
        //            Password = client.Password,
        //            FirstName = client.FirstName,
        //            LastName = client.LastName,
        //        };
        //        _clientRepository.Save(newClient);
        //        return Created("", newClient);
        //    }
        //    catch(Exception ex) {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        [HttpPost]
        public IActionResult NewClient([FromBody]NewClientDTO newClientDTO)
        {
            try
            {
                if(newClientDTO.FirstName.IsNullOrEmpty() || newClientDTO.LastName.IsNullOrEmpty() || newClientDTO.Email.IsNullOrEmpty() || newClientDTO.Password.IsNullOrEmpty())
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Missing fields");
                }
                Client client = _clientRepository.FindByEmail(newClientDTO.Email);
                if(client != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Email already in use");
                }
                Client newClient = new Client
                {
                    FirstName = newClientDTO.FirstName,
                    LastName = newClientDTO.LastName,
                    Email = newClientDTO.Email,
                    Password = newClientDTO.Password,
                };
                _clientRepository.Save(newClient);
                return StatusCode(201, new ClientDTO(newClientDTO));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }

}
