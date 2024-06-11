using HomeBankingAcc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingAcc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : Controller
    {
        private readonly ICardService _cardService;
        public CardsController(ICardService cardService)
        {
            _cardService = cardService;
        }
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAllCards()
        {
            try
            {
                var cardsDTO = _cardService.GetAllCards();
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
                var cardDTO = _cardService.FindById(id);
                return Ok(cardDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
