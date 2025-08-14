using System.ComponentModel.DataAnnotations;

namespace Marcador.Api.Dtos
{
    public class PuntosDto
    {
        [Required]
        public string Equipo { get; set; } = string.Empty; // "Local" | "Visitante"

        [Range(1, 3, ErrorMessage = "Los puntos deben ser 1, 2 o 3.")]
        public int Puntos { get; set; }
    }
}
