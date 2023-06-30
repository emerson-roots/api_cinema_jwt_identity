using A3_API_Project.Models.Cinema;
using A3_API_Project.Repository;
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
    public class LocalidadeController : ControllerBase
    {
        private readonly ILogger<LocalidadeController> _logger;
        private readonly LocalidadeRepository _repo;

        public LocalidadeController(ILogger<LocalidadeController> logger, LocalidadeRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        [HttpGet("GetAllRegioes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllRegioes()
        {

            try
            {
                var regioes = await _repo.GetAllRegioes();
                return Ok(regioes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = $"{ex}", StackTrace = $"{ ex.StackTrace }" });
            }
        }

        [HttpGet("GetAllCidades")]
        //[Authorize(Roles = "admin")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCidades()
        {

            try
            {
                var cidades = await _repo.GetAllCidades();
                return Ok(cidades);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = $"{ex}", StackTrace = $"{ ex.StackTrace }" });
            }
        }


        [HttpGet("GetCidadesPorRegiao/{idRegiao}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCidadesPorRegiao(int idRegiao)
        {

            try
            {
                var pergunta = await _repo.GetCidadesPorRegiao(idRegiao);
                return Ok(pergunta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = $"{ex}", StackTrace = $"{ ex.StackTrace }" });
            }
        }

        [HttpPost("CadastrarRegiao")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CadastrarRegiao(Regiao regiao)
        {

            try
            {
                throw new NotImplementedException("Endpoint ainda não foi implementado.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = $"{ex.Message}", StackTrace = $"{ ex }" });
            }
        }

        [HttpPost("CadastrarCidade")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CadastrarCidade(Cidade cidade)
        {

            try
            {
                throw new NotImplementedException("Endpoint ainda não foi implementado.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = $"{ex}", StackTrace = $"{ ex.StackTrace }" });
            }
        }



    }
}
