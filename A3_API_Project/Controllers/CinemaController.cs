using A3_API_Project.Models.Cinema.DTO;
using A3_API_Project.Repository;
using A3_API_Project.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace A3_API_Project.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CinemaController : ControllerBase
    {
        private readonly ILogger<CinemaController> _logger;
        private readonly CinemaRepository _repo;

        public CinemaController(ILogger<CinemaController> logger, CinemaRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        [HttpGet("GetCinemasPorCidade/{idCidade}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCinemasPorCidade(int idCidade)
        {
            try
            {
                var pergunta = await _repo.GetCinemasPorCidade(idCidade);
                return Ok(pergunta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = $"{ex}", StackTrace = $"{ ex.StackTrace }" });
            }
        }

        [HttpGet("GetCinemasPorRegiao/{idRegiao}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCinemasPorRegiao(int idRegiao)
        {
            try
            {
                var pergunta = await _repo.GetCinemasPorRegiao(idRegiao);
                return Ok(pergunta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = $"{ex}", StackTrace = $"{ ex.StackTrace }" });
            }
        }

        [HttpPost("CadastrarCinema")]
#if RELEASE
        [Authorize(Roles = "admin")]
#else
        [AllowAnonymous]
#endif
        public async Task<IActionResult> CadastrarCinema([FromBody] CinemaDTO cinemaDTO)
        {

            try
            {
                throw new NotImplementedException("Endpoint ainda não implementado...");
                var result = _repo.InsertCinemaAsync(cinemaDTO);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = $"{ex.Message}", StackTrace = $"{ ex }", Data = ex.Data });
            }
        }
    }
}
