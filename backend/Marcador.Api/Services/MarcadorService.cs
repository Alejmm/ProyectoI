using Marcador.Api.Models;

namespace Marcador.Api.Services
{
    public class MarcadorService
    {
        private MarcadorGlobal _marcador = new MarcadorGlobal
        {
            EquipoLocal = new Equipo { Nombre = "Local" },
            EquipoVisitante = new Equipo { Nombre = "Visitante" }
        };

        public MarcadorGlobal GetMarcador() => _marcador;

        public void SumarPuntos(string equipo, int puntos)
        {
            if (equipo == "Local") _marcador.EquipoLocal.Puntos += puntos;
            else _marcador.EquipoVisitante.Puntos += puntos;
        }

        public void RestarPuntos(string equipo, int puntos)
        {
            if (equipo == "Local") 
                _marcador.EquipoLocal.Puntos = Math.Max(0, _marcador.EquipoLocal.Puntos - puntos);
            else 
                _marcador.EquipoVisitante.Puntos = Math.Max(0, _marcador.EquipoVisitante.Puntos - puntos);
        }

        public void AvanzarCuarto()
        {
            _marcador.CuartoActual++;
            _marcador.TiempoRestante = 600; // reinicia tiempo
        }

        public void RegistrarFalta(string equipo)
        {
            if (equipo == "Local") _marcador.EquipoLocal.Faltas++;
            else _marcador.EquipoVisitante.Faltas++;
        }
    }
}
