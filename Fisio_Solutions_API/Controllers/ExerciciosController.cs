using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Fisio_Solutions_API.Services;

namespace Fisio_Solutions_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExerciciosController : ControllerBase
    {
        private readonly IExercicioService _exercicioService;

        public ExerciciosController(IExercicioService exercicioService)
        {
            _exercicioService = exercicioService;
        }

        /// <summary>
        /// Lista todos os exercícios ativos, com filtro opcional por região do corpo e indicador se está salvo pelo usuário.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? regiaoCorpo, [FromQuery] int? usuarioId)
        {
            // Identifica usuário pelo token ou query param
            var userId = usuarioId ?? GetUserIdFromClaims();

            var dtos = await _exercicioService.GetAllAsync(regiaoCorpo, userId);

            return Ok(dtos);
        }

        /// <summary>
        /// Obtém detalhes de um exercício específico por ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] int? usuarioId)
        {
            var userId = usuarioId ?? GetUserIdFromClaims();

            var exercicio = await _exercicioService.GetByIdAsync(id, userId);
            if (exercicio == null)
            {
                return NotFound(new { message = "Exercício não encontrado." });
            }

            return Ok(exercicio);
        }

        private int? GetUserIdFromClaims()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;

            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}

