using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fisio_Solutions_API.DTOs
{
    public class AdicionarItemListaRequest
    {
        [Required]
        public int ExercicioId { get; set; }

        public int? UsuarioId { get; set; }
        public string? NomeLista { get; set; }
        public string? Categoria { get; set; }
    }

    public class ExercicioDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string RegiaoCorpo { get; set; } = string.Empty;
        public string Objetivo { get; set; } = string.Empty;
        public string DescricaoPassoAPasso { get; set; } = string.Empty;
        public string ImagemIlustracaoUrl { get; set; } = string.Empty;
        public string RepeticaoSugerida { get; set; } = string.Empty;
        public int DuracaoSegundos { get; set; }
        public bool Ativo { get; set; }
        public bool IsSaved { get; set; }
        public int OrdemPosicao { get; set; }
    }

    public class ListaUsuarioDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NomeLista { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalExercicios { get; set; }
        public List<ExercicioDto> Exercicios { get; set; } = new();
    }
}

