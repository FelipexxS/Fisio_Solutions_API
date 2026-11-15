using Microsoft.Extensions.Caching.Memory;
using Fisio_Solutions_API.Models;

namespace Fisio_Solutions_API.Services
{
    /// <summary>
    /// Caretaker (GoF Memento): Gerencia a pilha (LIFO) de Mementos da ListaExercicios
    /// por usuário em memória, sem inspecionar nem alterar o conteúdo interno dos snapshots.
    /// </summary>
    public class ListaExerciciosCaretaker : IListaExerciciosCaretaker
    {
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan HistoryExpiration = TimeSpan.FromMinutes(30);

        public ListaExerciciosCaretaker(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void SaveState(int usuarioId, ListaExerciciosMemento memento)
        {
            ArgumentNullException.ThrowIfNull(memento);

            var cacheKey = GetCacheKey(usuarioId);
            var stack = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.SlidingExpiration = HistoryExpiration;
                return new Stack<ListaExerciciosMemento>();
            })!;

            stack.Push(memento);
        }

        public ListaExerciciosMemento? PopState(int usuarioId)
        {
            var cacheKey = GetCacheKey(usuarioId);

            if (_cache.TryGetValue(cacheKey, out Stack<ListaExerciciosMemento>? stack)
                && stack != null
                && stack.Count > 0)
            {
                return stack.Pop();
            }

            return null;
        }

        public bool HasHistory(int usuarioId)
        {
            var cacheKey = GetCacheKey(usuarioId);
            return _cache.TryGetValue(cacheKey, out Stack<ListaExerciciosMemento>? stack)
                && stack != null
                && stack.Count > 0;
        }

        private static string GetCacheKey(int usuarioId) => $"lista_memento_history:user:{usuarioId}";
    }
}

