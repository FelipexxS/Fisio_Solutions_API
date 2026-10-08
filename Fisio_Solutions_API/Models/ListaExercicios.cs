using System;
using System.Collections.Generic;

namespace Fisio_Solutions_API.Models
{
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
    }
}

