namespace Marcador.Api.Models
{
    public class MarcadorGlobal
    {
        public Equipo EquipoLocal { get; set; }
        public Equipo EquipoVisitante { get; set; }
        public int CuartoActual { get; set; } = 1;
        public int TiempoRestante { get; set; } = 600; // segundos, ejemplo 10 minutos
    }
}
