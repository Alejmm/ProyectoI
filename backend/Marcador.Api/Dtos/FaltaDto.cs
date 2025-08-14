using System.ComponentModel.DataAnnotations;

namespace Marcador.Api.Dtos
{
    public class FaltaDto
    {
        [Required]
        public string Equipo { get; set; } = string.Empty; // "Local" | "Visitante"
    }
}
