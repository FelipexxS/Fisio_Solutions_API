namespace Fisio_Solutions_API.Models
{
    public class ListaExercicioItem
    {
        public int Id { get; set; }
        public int ListaId { get; set; }
        public int ExercicioId { get; set; }
        public int OrdemPosicao { get; set; }

        // Relacionamento com Lista
        public ListaExercicios? Lista { get; set; }

        // Relacionamento com Exercício
        public Exercicio? Exercicio { get; set; }
    }
}

