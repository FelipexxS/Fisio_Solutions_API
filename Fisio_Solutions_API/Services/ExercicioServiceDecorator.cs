using Fisio_Solutions_API.DTOs;

namespace Fisio_Solutions_API.Services
{
    /// <summary>
    /// Decorator Base (GoF Decorator): Mantém uma referência ao componente IExercicioService
    /// envolvido e delega as chamadas por padrão para permitir extensão nas subclasses.
    /// </summary>
    public abstract class ExercicioServiceDecorator : IExercicioService
    {
        protected readonly IExercicioService _inner;

        protected ExercicioServiceDecorator(IExercicioService inner)
        {
            _inner = inner;
        }

        public virtual Task<List<ExercicioDto>> GetAllAsync(string? regiaoCorpo, int? usuarioId)
        {
            return _inner.GetAllAsync(regiaoCorpo, usuarioId);
        }

        public virtual Task<ExercicioDto?> GetByIdAsync(int id, int? usuarioId)
        {
            return _inner.GetByIdAsync(id, usuarioId);
        }
    }
}

