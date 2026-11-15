using Microsoft.Extensions.Caching.Memory;
using Fisio_Solutions_API.DTOs;

namespace Fisio_Solutions_API.Services
{
    /// <summary>
    /// ConcreteDecorator A (GoF Decorator): Adiciona comportamento de cache em memória
    /// ao catálogo de exercícios, evitando consultas repetidas ao banco de dados.
    /// </summary>
    public class CachedExercicioServiceDecorator : ExercicioServiceDecorator
    {
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public CachedExercicioServiceDecorator(IExercicioService inner, IMemoryCache cache)
            : base(inner)
        {
            _cache = cache;
        }

        public override async Task<List<ExercicioDto>> GetAllAsync(string? regiaoCorpo, int? usuarioId)
        {
            var normalizedRegion = string.IsNullOrWhiteSpace(regiaoCorpo)
                ? "all"
                : regiaoCorpo.Trim().ToLowerInvariant();

            var cacheKey = $"exercicios:list:{normalizedRegion}";

            var cachedList = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                return await base.GetAllAsync(regiaoCorpo, null);
            });

            // Retorna novas instâncias para evitar mutação compartilhada entre requisições/usuários
            return cachedList?.Select(CloneDto).ToList() ?? new List<ExercicioDto>();
        }

        public override async Task<ExercicioDto?> GetByIdAsync(int id, int? usuarioId)
        {
            var cacheKey = $"exercicios:id:{id}";

            var cachedItem = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                return await base.GetByIdAsync(id, null);
            });

            return cachedItem != null ? CloneDto(cachedItem) : null;
        }

        private static ExercicioDto CloneDto(ExercicioDto source)
        {
            return new ExercicioDto
            {
                Id = source.Id,
                Titulo = source.Titulo,
                RegiaoCorpo = source.RegiaoCorpo,
                Objetivo = source.Objetivo,
                DescricaoPassoAPasso = source.DescricaoPassoAPasso,
                ImagemIlustracaoUrl = source.ImagemIlustracaoUrl,
                RepeticaoSugerida = source.RepeticaoSugerida,
                DuracaoSegundos = source.DuracaoSegundos,
                Ativo = source.Ativo,
                IsSaved = source.IsSaved,
                OrdemPosicao = source.OrdemPosicao
            };
        }
    }
}

