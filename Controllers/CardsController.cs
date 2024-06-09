using HomeBankingAcc.DTOs;
using HomeBankingAcc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingAcc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : Controller
    {
        private readonly ICardRepository _cardRepository;
        public CardsController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAllCards()
        {
            try
            {
                var cards = _cardRepository.GetAllCards();
                var cardsDTO = cards.Select(c => new CardDTO(c)).ToList();
                return Ok(cardsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetCardById(long id)
        {
            try
            {
                var card = _cardRepository.FindById(id);
                var cardDTO = new CardDTO(card);
                return Ok(cardDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
