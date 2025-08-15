using System;
using Microsoft.EntityFrameworkCore;
using Marcador.Api.Models;
using Marcador.Api.Dtos;
using Timer = System.Timers.Timer;

namespace Marcador.Api.Services
{
    public class MarcadorService
    {
        private const int DURACION_CUARTO = 600;     // 10:00
        private const int DURACION_PRORROGA = 300;   // 5:00

        private readonly MarcadorDbContext _context;
        private readonly object _lock = new();
        private readonly Timer _timer;

        private bool _corriendo = false;
        private MarcadorGlobal _marcador;

        public MarcadorService(MarcadorDbContext context)
        {
            _context = context;

            // Cargar/crear marcador
            _marcador = _context.Marcadores
                .Include(m => m.EquipoLocal)
                .Include(m => m.EquipoVisitante)
                .FirstOrDefault() ?? new MarcadorGlobal
                {
                    EquipoLocal = new Equipo { Nombre = "Local", Puntos = 0, Faltas = 0, Jugadores = new() },
                    EquipoVisitante = new Equipo { Nombre = "Visitante", Puntos = 0, Faltas = 0, Jugadores = new() },
                    CuartoActual = 1,
                    TiempoRestante = DURACION_CUARTO,
                    EnProrroga = false,
                    NumeroProrroga = 0
                };

            if (_marcador.Id == 0)
            {
                _context.Marcadores.Add(_marcador);
                _context.SaveChanges();
            }
            else
            {
                if (_marcador.EquipoLocal == null) _marcador.EquipoLocal = new Equipo { Nombre = "Local", Puntos = 0, Faltas = 0, Jugadores = new() };
                if (_marcador.EquipoVisitante == null) _marcador.EquipoVisitante = new Equipo { Nombre = "Visitante", Puntos = 0, Faltas = 0, Jugadores = new() };
                if (_marcador.CuartoActual <= 0) _marcador.CuartoActual = 1;
                if (_marcador.TiempoRestante < 0) _marcador.TiempoRestante = 0;
                _context.SaveChanges();
            }

            // Timer (solo memoria, persistimos al pausar/acciones)
            _timer = new Timer(1000) { AutoReset = true, Enabled = false };
            _timer.Elapsed += (s, e) => DisminuirTiempo();
        }

        // ---- Lecturas ----
        public MarcadorGlobal GetMarcador() { lock (_lock) return _marcador; }

        public EstadoTiempoDto GetEstadoTiempo()
        {
            lock (_lock)
            {
                var dur = _marcador.EnProrroga ? DURACION_PRORROGA : DURACION_CUARTO;
                var estado = _corriendo ? "Running" : (_marcador.TiempoRestante == dur ? "Stopped" : "Paused");
                return new EstadoTiempoDto
                {
                    Estado = estado,
                    CuartoActual = _marcador.CuartoActual,
                    SegundosRestantes = _marcador.TiempoRestante,
                    DuracionCuarto = dur
                };
            }
        }

        // ---- Puntos ----
        public void SumarPuntos(string equipo, int puntos)
        {
            lock (_lock)
            {
                var eq = ObtenerEquipo(equipo);
                eq.Puntos += puntos;
                _context.SaveChanges();
            }
        }

        public void RestsarPuntos(string equipo, int puntos) => RestarPuntos(equipo, puntos); // alias si lo usas

        public void RestarPuntos(string equipo, int puntos)
        {
            lock (_lock)
            {
                var eq = ObtenerEquipo(equipo);
                eq.Puntos = Math.Max(0, eq.Puntos - puntos);
                _context.SaveChanges();
            }
        }

        // ---- Faltas ----
        public void RegistrarFalta(string equipo)
        {
            lock (_lock)
            {
                var eq = ObtenerEquipo(equipo);
                eq.Faltas += 1;
                _context.SaveChanges();
            }
        }

        // ---- Cuartos ----
        public void AvanzarCuarto()
        {
            lock (_lock)
            {
                _corriendo = false;
                _timer.Stop();

                if (_marcador.CuartoActual < 4)
                {
                    _marcador.CuartoActual++;
                    _marcador.EnProrroga = false;
                    _marcador.NumeroProrroga = 0;
                    _marcador.TiempoRestante = DURACION_CUARTO;
                }
                else
                {
                    _marcador.EnProrroga = true;
                    _marcador.NumeroProrroga++;
                    _marcador.TiempoRestante = DURACION_PRORROGA;
                }

                // reset de faltas por equipo al cambiar de periodo
                _marcador.EquipoLocal.Faltas = 0;
                _marcador.EquipoVisitante.Faltas = 0;

                _context.SaveChanges();
            }
        }

        // ---- Tiempo ----
        public void IniciarTiempo()
        {
            lock (_lock)
            {
                if (_corriendo) return;
                _corriendo = true;
                _timer.Start();
            }
        }

        public void PausarTiempo()
        {
            lock (_lock)
            {
                if (!_corriendo) return;
                _corriendo = false;
                _timer.Stop();
                _context.SaveChanges();
            }
        }

        public void ReanudarTiempo() => IniciarTiempo();

        public MarcadorGlobal ReiniciarTiempo(int segundos = DURACION_CUARTO)
        {
            lock (_lock)
            {
                _corriendo = false;
                _timer.Stop();
                if (segundos < 0) segundos = 0;
                _marcador.TiempoRestante = segundos;
                _context.SaveChanges();
                return _marcador;
            }
        }

        public MarcadorGlobal EstablecerTiempo(int segundos)
        {
            lock (_lock)
            {
                if (segundos < 0) segundos = 0;
                _marcador.TiempoRestante = segundos;
                _context.SaveChanges();
                return _marcador;
            }
        }

        // ---- Tick ----
        private void DisminuirTiempo()
        {
            lock (_lock)
            {
                if (!_corriendo) return;
                if (_marcador.TiempoRestante > 0)
                {
                    _marcador.TiempoRestante--;
                }
                else
                {
                    _corriendo = false;
                    _timer.Stop();
                    _context.SaveChanges();
                }
            }
        }

        // ---- Helper ----
        private Equipo ObtenerEquipo(string equipo)
        {
            if (string.Equals(equipo, "Local", StringComparison.OrdinalIgnoreCase)) return _marcador.EquipoLocal;
            if (string.Equals(equipo, "Visitante", StringComparison.OrdinalIgnoreCase)) return _marcador.EquipoVisitante;
            throw new ArgumentException("El equipo debe ser 'Local' o 'Visitante'.");
        }
    }
}
