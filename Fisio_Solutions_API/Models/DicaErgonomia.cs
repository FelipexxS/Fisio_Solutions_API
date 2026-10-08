using System;

namespace Fisio_Solutions_API.Models
{
    public class DicaErgonomia
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Conteudo { get; set; } = string.Empty;
        public string? ImagemIlustracaoUrl { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

