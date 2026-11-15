using System;
using System.Collections.Generic;
using System.Linq;

namespace Fisio_Solutions_API.Models
{
    /// <summary>
    /// Representa o estado imutável de um item da lista dentro do Memento.
    /// </summary>
    public sealed record ListaExercicioItemSnapshot(int ExercicioId, int OrdemPosicao);

    /// <summary>
    /// Memento (GoF Memento): Armazena de forma imutável o estado interno (snapshot)
    /// de uma instância de ListaExercicios em um determinado momento no tempo.
    /// </summary>
    public sealed class ListaExerciciosMemento
    {
        public string NomeLista { get; }
        public string? Categoria { get; }
        public IReadOnlyList<ListaExercicioItemSnapshot> Itens { get; }
        public DateTime SavedAt { get; }

        public ListaExerciciosMemento(
            string nomeLista,
            string? categoria,
            IEnumerable<ListaExercicioItemSnapshot> itens)
        {
            NomeLista = nomeLista;
            Categoria = categoria;
            Itens = itens.ToList().AsReadOnly();
            SavedAt = DateTime.UtcNow;
        }
    }
}

