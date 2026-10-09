using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Fisio_Solutions_API.Data;
using Fisio_Solutions_API.DTOs;

namespace Fisio_Solutions_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExerciciosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExerciciosController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os exercícios ativos, com filtro opcional por região do corpo e indicador se está salvo pelo usuário.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? regiaoCorpo, [FromQuery] int? usuarioId)
        {
            var query = _context.Exercicios.Where(e => e.Ativo);

            if (!string.IsNullOrWhiteSpace(regiaoCorpo))
            {
                var regiaoNormalizada = regiaoCorpo.Trim().ToLower();
                query = query.Where(e => e.RegiaoCorpo.ToLower() == regiaoNormalizada);
            }

            var exercicios = await query.ToListAsync();

            // Identifica usuário pelo token ou query param
            var userId = usuarioId ?? GetUserIdFromClaims();
            var savedExerciseIds = new HashSet<int>();

            if (userId.HasValue)
            {
                var userList = await _context.ListasExercicios
                    .Include(l => l.Itens)
                    .FirstOrDefaultAsync(l => l.UsuarioId == userId.Value);

                if (userList != null)
                {
                    savedExerciseIds = userList.Itens.Select(i => i.ExercicioId).ToHashSet();
                }
            }

            var dtos = exercicios.Select(e => new ExercicioDto
            {
                Id = e.Id,
                Titulo = e.Titulo,
                RegiaoCorpo = e.RegiaoCorpo,
                Objetivo = e.Objetivo,
                DescricaoPassoAPasso = e.DescricaoPassoAPasso,
                ImagemIlustracaoUrl = e.ImagemIlustracaoUrl,
                RepeticaoSugerida = e.RepeticaoSugerida,
                DuracaoSegundos = e.DuracaoSegundos,
                Ativo = e.Ativo,
                IsSaved = savedExerciseIds.Contains(e.Id)
            }).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Obtém detalhes de um exercício específico por ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] int? usuarioId)
        {
            var exercicio = await _context.Exercicios.FindAsync(id);
            if (exercicio == null || !exercicio.Ativo)
            {
                return NotFound(new { message = "Exercício não encontrado." });
            }

            var userId = usuarioId ?? GetUserIdFromClaims();
            var isSaved = false;

            if (userId.HasValue)
            {
                var userList = await _context.ListasExercicios
                    .Include(l => l.Itens)
                    .FirstOrDefaultAsync(l => l.UsuarioId == userId.Value);

                if (userList != null)
                {
                    isSaved = userList.Itens.Any(i => i.ExercicioId == id);
                }
            }

            return Ok(new ExercicioDto
            {
                Id = exercicio.Id,
                Titulo = exercicio.Titulo,
                RegiaoCorpo = exercicio.RegiaoCorpo,
                Objetivo = exercicio.Objetivo,
                DescricaoPassoAPasso = exercicio.DescricaoPassoAPasso,
                ImagemIlustracaoUrl = exercicio.ImagemIlustracaoUrl,
                RepeticaoSugerida = exercicio.RepeticaoSugerida,
                DuracaoSegundos = exercicio.DuracaoSegundos,
                Ativo = exercicio.Ativo,
                IsSaved = isSaved
            });
        }

        private int? GetUserIdFromClaims()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;

            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}

