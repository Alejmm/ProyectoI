namespace Marcador.Api.Models
{
    public class MarcadorGlobal
    {public int Id { get; set; }

    public int EquipoLocalId { get; set; }
    public Equipo EquipoLocal { get; set; }

    public int EquipoVisitanteId { get; set; }
    public Equipo EquipoVisitante { get; set; }

    public int CuartoActual { get; set; } = 1;
    public int TiempoRestante { get; set; } = 600; 
    }
}
