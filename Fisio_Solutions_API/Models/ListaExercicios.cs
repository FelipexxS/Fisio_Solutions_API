using System;
using System.Collections.Generic;
using System.Linq;

namespace Fisio_Solutions_API.Models
{
    /// <summary>
    /// Originator (GoF Memento): Entidade de domínio que representa a lista de exercícios do usuário
    /// e sabe criar e restaurar snapshots (Mementos) do seu estado interno.
    /// </summary>
    public class ListaExercicios
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NomeLista { get; set; } = "Minha Lista de Exercícios";
        public string? Categoria { get; set; } // "Coluna", "Pescoço", "Braços", "Pernas"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relacionamento com Usuário
        public User? Usuario { get; set; }

        // Relacionamento 1:N com Lista_Exercicio_Itens
        public ICollection<ListaExercicioItem> Itens { get; set; } = new List<ListaExercicioItem>();

        /// <summary>
        /// Captura o estado atual da lista e de seus itens em um objeto Memento imutável.
        /// </summary>
        public ListaExerciciosMemento SaveToMemento()
        {
            var itensSnapshot = Itens
                .OrderBy(i => i.OrdemPosicao)
                .Select(i => new ListaExercicioItemSnapshot(i.ExercicioId, i.OrdemPosicao))
                .ToList();

            return new ListaExerciciosMemento(NomeLista, Categoria, itensSnapshot);
        }

        /// <summary>
        /// Restaura o estado da lista e de seus itens a partir de um Memento salvo anteriormente.
        /// </summary>
        public void RestoreFromMemento(ListaExerciciosMemento memento)
        {
            ArgumentNullException.ThrowIfNull(memento);

            NomeLista = memento.NomeLista;
            Categoria = memento.Categoria;

            Itens.Clear();

            foreach (var itemSnapshot in memento.Itens.OrderBy(i => i.OrdemPosicao))
            {
                Itens.Add(new ListaExercicioItem
                {
                    ListaId = Id,
                    ExercicioId = itemSnapshot.ExercicioId,
                    OrdemPosicao = itemSnapshot.OrdemPosicao
                });
            }
        }
    }
}

