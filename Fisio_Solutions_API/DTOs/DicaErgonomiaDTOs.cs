using System;
using System.ComponentModel.DataAnnotations;

namespace Fisio_Solutions_API.DTOs
{
    public class CriarDicaErgonomiaDto
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(250, ErrorMessage = "O título deve ter no máximo 250 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O conteúdo é obrigatório.")]
        public string Conteudo { get; set; } = string.Empty;

        public string? ImagemIlustracaoUrl { get; set; }
    }

    public class AtualizarDicaErgonomiaDto
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(250, ErrorMessage = "O título deve ter no máximo 250 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O conteúdo é obrigatório.")]
        public string Conteudo { get; set; } = string.Empty;

        public string? ImagemIlustracaoUrl { get; set; }

        public bool Ativo { get; set; } = true;
    }

    public class DicaErgonomiaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Conteudo { get; set; } = string.Empty;
        public string? ImagemIlustracaoUrl { get; set; }
        public bool Ativo { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

