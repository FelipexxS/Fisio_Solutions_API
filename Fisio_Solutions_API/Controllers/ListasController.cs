using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Fisio_Solutions_API.Data;
using Fisio_Solutions_API.DTOs;
using Fisio_Solutions_API.Models;

namespace Fisio_Solutions_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ListasController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Endpoint POST /api/listas para associar novos exercícios à lista do usuário.
        /// Cria a lista caso o usuário ainda não possua uma, ou adiciona à lista existente.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AdicionarExercicio([FromBody] AdicionarItemListaRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = request.UsuarioId ?? GetUserIdFromClaims();
            if (!userId.HasValue)
            {
                // Fallback para primeiro usuário ou usuário padrão de demonstração
                var firstUser = await _context.Users.FirstOrDefaultAsync();
                if (firstUser == null)
                {
                    firstUser = new User
                    {
                        FullName = "Paciente Demonstração",
                        Email = "paciente@fisiosolutions.com",
                        Profession = "Paciente",
                        BirthDate = "01/01/1990",
                        PasswordHash = "demo",
                        PasswordSalt = "demo"
                    };
                    _context.Users.Add(firstUser);
                    await _context.SaveChangesAsync();
                }
                userId = firstUser.Id;
            }

            // Verifica se o exercício existe
            var exercicio = await _context.Exercicios.FindAsync(request.ExercicioId);
            if (exercicio == null || !exercicio.Ativo)
            {
                return NotFound(new { message = "Exercício não encontrado." });
            }

            // Obtém ou cria a lista do usuário (relação: usuário possui uma lista de exercícios)
            var lista = await _context.ListasExercicios
                .Include(l => l.Itens)
                .FirstOrDefaultAsync(l => l.UsuarioId == userId.Value);

            if (lista == null)
            {
                lista = new ListaExercicios
                {
                    UsuarioId = userId.Value,
                    NomeLista = request.NomeLista ?? "Minhas Listas de Exercícios",
                    Categoria = request.Categoria,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ListasExercicios.Add(lista);
                await _context.SaveChangesAsync();
            }

            // Verifica se o exercício já está associado à lista
            var itemExistente = lista.Itens.FirstOrDefault(i => i.ExercicioId == request.ExercicioId);
            if (itemExistente != null)
            {
                return Ok(new
                {
                    message = "Exercício já está salvo na lista do usuário.",
                    listaId = lista.Id,
                    exercicioId = request.ExercicioId,
                    totalSalvos = lista.Itens.Count
                });
            }

            // Cria o novo item na lista na próxima posição ordinal
            var proximaPosicao = lista.Itens.Count + 1;
            var novoItem = new ListaExercicioItem
            {
                ListaId = lista.Id,
                ExercicioId = request.ExercicioId,
                OrdemPosicao = proximaPosicao
            };

            _context.ListaExercicioItens.Add(novoItem);
            await _context.SaveChangesAsync();

            var totalSalvos = await _context.ListaExercicioItens.CountAsync(i => i.ListaId == lista.Id);

            return CreatedAtAction(nameof(GetMinhaLista), new { usuarioId = userId.Value }, new
            {
                message = "Exercício associado com sucesso à lista do usuário.",
                listaId = lista.Id,
                exercicioId = request.ExercicioId,
                totalSalvos = totalSalvos
            });
        }

        /// <summary>
        /// Obtém a lista de exercícios do usuário logado ou especificado com todos os itens salvos.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMinhaLista([FromQuery] int? usuarioId)
        {
            var userId = usuarioId ?? GetUserIdFromClaims();
            if (!userId.HasValue)
            {
                var firstUser = await _context.Users.FirstOrDefaultAsync();
                userId = firstUser?.Id;
            }

            if (!userId.HasValue)
            {
                return Ok(new ListaUsuarioDto
                {
                    NomeLista = "Minhas Listas de Exercícios",
                    TotalExercicios = 0,
                    Exercicios = new List<ExercicioDto>()
                });
            }

            var lista = await _context.ListasExercicios
                .Include(l => l.Itens)
                    .ThenInclude(i => i.Exercicio)
                .FirstOrDefaultAsync(l => l.UsuarioId == userId.Value);

            if (lista == null)
            {
                return Ok(new ListaUsuarioDto
                {
                    UsuarioId = userId.Value,
                    NomeLista = "Minhas Listas de Exercícios",
                    TotalExercicios = 0,
                    Exercicios = new List<ExercicioDto>()
                });
            }

            var itensOrdenados = lista.Itens
                .OrderBy(i => i.OrdemPosicao)
                .Where(i => i.Exercicio != null && i.Exercicio.Ativo)
                .Select(i => new ExercicioDto
                {
                    Id = i.Exercicio!.Id,
                    Titulo = i.Exercicio.Titulo,
                    RegiaoCorpo = i.Exercicio.RegiaoCorpo,
                    Objetivo = i.Exercicio.Objetivo,
                    DescricaoPassoAPasso = i.Exercicio.DescricaoPassoAPasso,
                    ImagemIlustracaoUrl = i.Exercicio.ImagemIlustracaoUrl,
                    RepeticaoSugerida = i.Exercicio.RepeticaoSugerida,
                    DuracaoSegundos = i.Exercicio.DuracaoSegundos,
                    Ativo = i.Exercicio.Ativo,
                    IsSaved = true,
                    OrdemPosicao = i.OrdemPosicao
                }).ToList();

            var dto = new ListaUsuarioDto
            {
                Id = lista.Id,
                UsuarioId = lista.UsuarioId,
                NomeLista = lista.NomeLista,
                Categoria = lista.Categoria,
                CreatedAt = lista.CreatedAt,
                TotalExercicios = itensOrdenados.Count,
                Exercicios = itensOrdenados
            };

            return Ok(dto);
        }

        /// <summary>
        /// Remove um exercício da lista do usuário ("Deletar" / "Remover").
        /// </summary>
        [HttpDelete("itens/{exercicioId}")]
        [HttpDelete("{exercicioId}")]
        public async Task<IActionResult> RemoverExercicio(int exercicioId, [FromQuery] int? usuarioId)
        {
            var userId = usuarioId ?? GetUserIdFromClaims();
            if (!userId.HasValue)
            {
                var firstUser = await _context.Users.FirstOrDefaultAsync();
                userId = firstUser?.Id;
            }

            if (!userId.HasValue)
            {
                return NotFound(new { message = "Lista não encontrada para o usuário." });
            }

            var lista = await _context.ListasExercicios
                .Include(l => l.Itens)
                .FirstOrDefaultAsync(l => l.UsuarioId == userId.Value);

            if (lista == null)
            {
                return NotFound(new { message = "Lista não encontrada." });
            }

            var item = lista.Itens.FirstOrDefault(i => i.ExercicioId == exercicioId);
            if (item == null)
            {
                return NotFound(new { message = "Exercício não encontrado na lista." });
            }

            _context.ListaExercicioItens.Remove(item);
            await _context.SaveChangesAsync();

            var totalRestantes = await _context.ListaExercicioItens.CountAsync(i => i.ListaId == lista.Id);

            return Ok(new
            {
                message = "Exercício removido da lista com sucesso.",
                exercicioId = exercicioId,
                totalRestantes = totalRestantes
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

