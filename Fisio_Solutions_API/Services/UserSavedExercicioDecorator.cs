using Microsoft.EntityFrameworkCore;
using Fisio_Solutions_API.Data;
using Fisio_Solutions_API.DTOs;

namespace Fisio_Solutions_API.Services
{
    /// <summary>
    /// ConcreteDecorator B (GoF Decorator): Enriquece os DTOs de exercícios indicando
    /// se cada exercício está salvo na lista personalizada do usuário (IsSaved).
    /// </summary>
    public class UserSavedExercicioDecorator : ExercicioServiceDecorator
    {
        private readonly AppDbContext _context;

        public UserSavedExercicioDecorator(IExercicioService inner, AppDbContext context)
            : base(inner)
        {
            _context = context;
        }

        public override async Task<List<ExercicioDto>> GetAllAsync(string? regiaoCorpo, int? usuarioId)
        {
            var exercicios = await base.GetAllAsync(regiaoCorpo, usuarioId);

            if (!usuarioId.HasValue || exercicios.Count == 0)
            {
                return exercicios;
            }

            var savedExerciseIds = new HashSet<int>();

            var userList = await _context.ListasExercicios
                .Include(l => l.Itens)
                .FirstOrDefaultAsync(l => l.UsuarioId == usuarioId.Value);

            if (userList != null)
            {
                savedExerciseIds = userList.Itens.Select(i => i.ExercicioId).ToHashSet();
            }

            foreach (var exercicio in exercicios)
            {
                exercicio.IsSaved = savedExerciseIds.Contains(exercicio.Id);
            }

            return exercicios;
        }

        public override async Task<ExercicioDto?> GetByIdAsync(int id, int? usuarioId)
        {
            var exercicio = await base.GetByIdAsync(id, usuarioId);

            if (exercicio == null || !usuarioId.HasValue)
            {
                return exercicio;
            }

            var userList = await _context.ListasExercicios
                .Include(l => l.Itens)
                .FirstOrDefaultAsync(l => l.UsuarioId == usuarioId.Value);

            if (userList != null)
            {
                exercicio.IsSaved = userList.Itens.Any(i => i.ExercicioId == id);
            }

            return exercicio;
        }
    }
}

