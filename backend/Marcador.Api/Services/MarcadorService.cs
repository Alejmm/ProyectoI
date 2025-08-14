using System;
using Marcador.Api.Models;

namespace Marcador.Api.Services
{
    public class MarcadorService
    {
        private const int DURACION_CUARTO = 600;     // 10:00
        private const int DURACION_PRORROGA = 300;   // 5:00

        private readonly object _lock = new();

        private MarcadorGlobal _marcador = new MarcadorGlobal
        {
            EquipoLocal = new Equipo { Nombre = "Local", Puntos = 0, Faltas = 0, Jugadores = new() },
            EquipoVisitante = new Equipo { Nombre = "Visitante", Puntos = 0, Faltas = 0, Jugadores = new() },
            CuartoActual = 1,
            TiempoRestante = DURACION_CUARTO,
            EnProrroga = false,
            NumeroProrroga = 0
        };

        public MarcadorGlobal GetMarcador()
        {
            lock (_lock) return _marcador;
        }

        public void SumarPuntos(string equipo, int puntos)
        {
            lock (_lock)
            {
                if (string.Equals(equipo, "Local", StringComparison.OrdinalIgnoreCase))
                    _marcador.EquipoLocal.Puntos += puntos;
                else
                    _marcador.EquipoVisitante.Puntos += puntos;
            }
        }

        public void RestarPuntos(string equipo, int puntos)
        {
            lock (_lock)
            {
                if (string.Equals(equipo, "Local", StringComparison.OrdinalIgnoreCase))
                    _marcador.EquipoLocal.Puntos = Math.Max(0, _marcador.EquipoLocal.Puntos - puntos);
                else
                    _marcador.EquipoVisitante.Puntos = Math.Max(0, _marcador.EquipoVisitante.Puntos - puntos);
            }
        }

        public void RegistrarFalta(string equipo)
        {
            lock (_lock)
            {
                if (string.Equals(equipo, "Local", StringComparison.OrdinalIgnoreCase))
                    _marcador.EquipoLocal.Faltas++;
                else
                    _marcador.EquipoVisitante.Faltas++;
            }
        }

        public void AvanzarCuarto()
        {
            lock (_lock)
            {
                // Avanza periodos: 1..4 → prórroga(s)
                if (_marcador.CuartoActual < 4)
                {
                    _marcador.CuartoActual++;
                    _marcador.EnProrroga = false;
                    _marcador.NumeroProrroga = 0;
                }
                else
                {
                    _marcador.EnProrroga = true;
                    _marcador.NumeroProrroga++;
                }

                // Reloj según fase
                _marcador.TiempoRestante = _marcador.EnProrroga ? DURACION_PRORROGA : DURACION_CUARTO;

                // Reinicia faltas por equipo en cada periodo
                _marcador.EquipoLocal.Faltas = 0;
                _marcador.EquipoVisitante.Faltas = 0;
            }
        }

        public MarcadorGlobal EstablecerTiempo(int segundos)
        {
            lock (_lock)
            {
                if (segundos < 0) segundos = 0;
                _marcador.TiempoRestante = segundos;
                return _marcador;
            }
        }

        public MarcadorGlobal ReiniciarTiempo(int segundos = DURACION_CUARTO)
        {
            lock (_lock)
            {
                if (segundos < 0) segundos = 0;
                _marcador.TiempoRestante = segundos;
                return _marcador;
            }
        }
    }
}
