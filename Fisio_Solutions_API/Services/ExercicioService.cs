using Microsoft.EntityFrameworkCore;
using Fisio_Solutions_API.Data;
using Fisio_Solutions_API.DTOs;

namespace Fisio_Solutions_API.Services
{
    /// <summary>
    /// ConcreteComponent (GoF Decorator): Implementação base responsável apenas por
    /// consultar os exercícios ativos no banco de dados e convertê-los em DTOs.
    /// </summary>
    public class ExercicioService : IExercicioService
    {
        private readonly AppDbContext _context;

        public ExercicioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExercicioDto>> GetAllAsync(string? regiaoCorpo, int? usuarioId)
        {
            var query = _context.Exercicios.Where(e => e.Ativo);

            if (!string.IsNullOrWhiteSpace(regiaoCorpo))
            {
                var regiaoNormalizada = regiaoCorpo.Trim().ToLower();
                query = query.Where(e => e.RegiaoCorpo.ToLower() == regiaoNormalizada);
            }

            var exercicios = await query.ToListAsync();

            return exercicios.Select(e => new ExercicioDto
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
                IsSaved = false
            }).ToList();
        }

        public async Task<ExercicioDto?> GetByIdAsync(int id, int? usuarioId)
        {
            var exercicio = await _context.Exercicios.FindAsync(id);
            if (exercicio == null || !exercicio.Ativo)
            {
                return null;
            }

            return new ExercicioDto
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
                IsSaved = false
            };
        }
    }
}

