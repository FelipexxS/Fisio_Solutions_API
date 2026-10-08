using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fisio_Solutions_API.Data;
using Fisio_Solutions_API.DTOs;
using Fisio_Solutions_API.Models;

namespace Fisio_Solutions_API.Controllers
{
    [ApiController]
    [Route("api/dicas-ergonomia")]
    public class DicasErgonomiaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DicasErgonomiaController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todas as dicas de ergonomia ativas da plataforma.
        /// GET /api/dicas-ergonomia
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dicas = await _context.DicasErgonomia
                .Where(d => d.Ativo)
                .OrderBy(d => d.Id)
                .Select(d => new DicaErgonomiaDto
                {
                    Id = d.Id,
                    Titulo = d.Titulo,
                    Conteudo = d.Conteudo,
                    ImagemIlustracaoUrl = d.ImagemIlustracaoUrl,
                    Ativo = d.Ativo,
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();

            return Ok(dicas);
        }

        /// <summary>
        /// Obtém detalhes de uma dica de ergonomia específica por ID.
        /// GET /api/dicas-ergonomia/{dicaErgonomiaId}
        /// </summary>
        [HttpGet("{dicaErgonomiaId}")]
        public async Task<IActionResult> GetById(int dicaErgonomiaId)
        {
            var dica = await _context.DicasErgonomia.FindAsync(dicaErgonomiaId);
            if (dica == null || !dica.Ativo)
            {
                return NotFound(new { message = "Dica de ergonomia não encontrada." });
            }

            return Ok(new DicaErgonomiaDto
            {
                Id = dica.Id,
                Titulo = dica.Titulo,
                Conteudo = dica.Conteudo,
                ImagemIlustracaoUrl = dica.ImagemIlustracaoUrl,
                Ativo = dica.Ativo,
                CreatedAt = dica.CreatedAt
            });
        }

        /// <summary>
        /// Associa/Cadastra uma nova dica de ergonomia à plataforma.
        /// POST /api/dicas-ergonomia
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CriarDicaErgonomiaDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var novaDica = new DicaErgonomia
            {
                Titulo = request.Titulo.Trim(),
                Conteudo = request.Conteudo.Trim(),
                ImagemIlustracaoUrl = string.IsNullOrWhiteSpace(request.ImagemIlustracaoUrl)
                    ? "ergonomics_banner"
                    : request.ImagemIlustracaoUrl.Trim(),
                Ativo = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.DicasErgonomia.Add(novaDica);
            await _context.SaveChangesAsync();

            var dto = new DicaErgonomiaDto
            {
                Id = novaDica.Id,
                Titulo = novaDica.Titulo,
                Conteudo = novaDica.Conteudo,
                ImagemIlustracaoUrl = novaDica.ImagemIlustracaoUrl,
                Ativo = novaDica.Ativo,
                CreatedAt = novaDica.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { dicaErgonomiaId = novaDica.Id }, dto);
        }

        /// <summary>
        /// Atualiza os dados de uma dica de ergonomia existente.
        /// PUT /api/dicas-ergonomia/{dicaErgonomiaId}
        /// </summary>
        [HttpPut("{dicaErgonomiaId}")]
        public async Task<IActionResult> Update(int dicaErgonomiaId, [FromBody] AtualizarDicaErgonomiaDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dica = await _context.DicasErgonomia.FindAsync(dicaErgonomiaId);
            if (dica == null)
            {
                return NotFound(new { message = "Dica de ergonomia não encontrada para atualização." });
            }

            dica.Titulo = request.Titulo.Trim();
            dica.Conteudo = request.Conteudo.Trim();
            if (!string.IsNullOrWhiteSpace(request.ImagemIlustracaoUrl))
            {
                dica.ImagemIlustracaoUrl = request.ImagemIlustracaoUrl.Trim();
            }
            dica.Ativo = request.Ativo;

            await _context.SaveChangesAsync();

            var dto = new DicaErgonomiaDto
            {
                Id = dica.Id,
                Titulo = dica.Titulo,
                Conteudo = dica.Conteudo,
                ImagemIlustracaoUrl = dica.ImagemIlustracaoUrl,
                Ativo = dica.Ativo,
                CreatedAt = dica.CreatedAt
            };

            return Ok(dto);
        }

        /// <summary>
        /// Remove uma dica de ergonomia pelo ID.
        /// DELETE /api/dicas-ergonomia/{dicaErgonomiaId}
        /// </summary>
        [HttpDelete("{dicaErgonomiaId}")]
        public async Task<IActionResult> Delete(int dicaErgonomiaId)
        {
            var dica = await _context.DicasErgonomia.FindAsync(dicaErgonomiaId);
            if (dica == null)
            {
                return NotFound(new { message = "Dica de ergonomia não encontrada para remoção." });
            }

            _context.DicasErgonomia.Remove(dica);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Dica de ergonomia removida com sucesso.", id = dicaErgonomiaId });
        }
    }
}

