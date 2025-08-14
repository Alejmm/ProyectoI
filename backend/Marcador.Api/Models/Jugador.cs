namespace Marcador.Api.Models

{
    public class Jugador
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Puntos { get; set; }
        public int Faltas { get; set; } 

        public int EquipoId { get; set; }
        public Equipo Equipo { get; set; }
       
     } 

 }
