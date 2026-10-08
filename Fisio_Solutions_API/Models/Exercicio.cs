namespace Fisio_Solutions_API.Models
{
    public class Exercicio
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string RegiaoCorpo { get; set; } = string.Empty; // "Coluna", "Pescoço", "Braços", "Pernas"
        public string Objetivo { get; set; } = string.Empty; // "alongamento", "força"
        public string DescricaoPassoAPasso { get; set; } = string.Empty;
        public string ImagemIlustracaoUrl { get; set; } = string.Empty;
        public string RepeticaoSugerida { get; set; } = string.Empty;
        public int DuracaoSegundos { get; set; } = 30;
        public bool Ativo { get; set; } = true;

        // Propriedade de navegação inversa
        public ICollection<ListaExercicioItem> ListaItens { get; set; } = new List<ListaExercicioItem>();
    }
}

