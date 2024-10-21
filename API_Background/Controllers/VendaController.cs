using API_Background.Exceptions;
using API_Background.Models;
using API_Background.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_Background.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendaController(IVendaService vendaService) : ControllerBase
    {
        private readonly IVendaService _vendaService = vendaService;

        [HttpPost("criar-venda")]
        public async Task<IActionResult> CriarVendaAsync([FromBody] Venda venda)
        {
            try
            {
                await _vendaService.CriarVendaAsync(venda);
            }
            catch (ImputException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created(); 
        }

        [HttpGet("vendas")]
        public async Task<IActionResult> ObterTodasVendas()
        {
            try
            {
                var vendas = await _vendaService.ObterTodasVendas();

                return Ok(vendas);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("vendas-validas-processadas")]
        public async Task<IActionResult> ObterCarrosComVendasValidasProcessadas()
        {
            try
            {
                var vendas = await _vendaService.ObterVendasValidasProcessadas();

                return Ok(vendas);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("vendas-validas-nao-processadas")]
        public async Task<IActionResult> ObterCarrosComVendasValidasNaoProcessadas()
        {
            try
            {
                var vendas = await _vendaService.ObterVendasValidasNaoProcessadas();

                return Ok(vendas);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
