using System.Collections.Generic;

using Marcador.Api.Models; // <- Esto permite usar Jugador
namespace Marcador.Api.Models
{
    public class Equipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Puntos { get; set; }
        public int Faltas { get; set; }
        public List<Jugador> Jugadores { get; set; } = new();
    }
}   