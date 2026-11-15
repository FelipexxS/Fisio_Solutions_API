using Fisio_Solutions_API.DTOs;

namespace Fisio_Solutions_API.Services
{
    /// <summary>
    /// Component (GoF Decorator): Define o contrato para consulta de exercícios.
    /// </summary>
    public interface IExercicioService
    {
        Task<List<ExercicioDto>> GetAllAsync(string? regiaoCorpo, int? usuarioId);
        Task<ExercicioDto?> GetByIdAsync(int id, int? usuarioId);
    }
}

