namespace Marcador.Api.Models
{
    public class Falta
    {
        public int Id { get; set; }
        public int JugadorId { get; set; }
        public int EquipoId { get; set; }
        public string Tipo { get; set; } // personal, técnica, etc.
        public int Minuto { get; set; }
    }
}
