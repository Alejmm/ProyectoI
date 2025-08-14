using System;
using Microsoft.EntityFrameworkCore;
using Marcador.Api.Models;
using Marcador.Api.Dtos;
using Timer = System.Timers.Timer; // usa el Timer de System.Timers

namespace Marcador.Api.Services
{
    public class MarcadorService
    {
        private readonly MarcadorDbContext _context;

        // Estado en memoria
        private MarcadorGlobal _marcador = null!;
        private bool _corriendo = false;

        // Duración por cuarto (configurable). 10 min por defecto.
        private const int DEFAULT_DURACION_POR_CUARTO_SEG = 600;
        private int _duracionPorCuartoSeg = DEFAULT_DURACION_POR_CUARTO_SEG;

        // Timer que descuenta 1 segundo (solo cambia memoria)
        private Timer _timer = null!;

        public MarcadorService(MarcadorDbContext context)
        {
            _context = context;

            // Cargar desde la BD
            _marcador = _context.Marcadores
                .Include(m => m.EquipoLocal)
                .Include(m => m.EquipoVisitante)
                .FirstOrDefault();

            // Si no existe, crear con valores por defecto
            if (_marcador == null)
            {
                _marcador = new MarcadorGlobal
                {
                    EquipoLocal = new Equipo { Nombre = "Local", Puntos = 0, Faltas = 0 },
                    EquipoVisitante = new Equipo { Nombre = "Visitante", Puntos = 0, Faltas = 0 },
                    CuartoActual = 1,
                    TiempoRestante = _duracionPorCuartoSeg
                };

                _context.Marcadores.Add(_marcador);
                _context.SaveChanges();
            }
            else
            {
                // 
                if (_marcador.EquipoLocal == null)
                    _marcador.EquipoLocal = new Equipo { Nombre = "Local", Puntos = 0, Faltas = 0 };

                if (_marcador.EquipoVisitante == null)
                    _marcador.EquipoVisitante = new Equipo { Nombre = "Visitante", Puntos = 0, Faltas = 0 };

                if (_marcador.CuartoActual <= 0)
                    _marcador.CuartoActual = 1;

                if (_marcador.TiempoRestante <= 0)
                    _marcador.TiempoRestante = _duracionPorCuartoSeg;

                _context.SaveChanges();
            }

            // Configurar Timer (no guarda en BD, solo memoria)
            _timer = new Timer(1000);   // 1000 ms = 1 s
            _timer.AutoReset = true;    // se repite
            _timer.Enabled = false;     // inicia apagado
            _timer.Elapsed += (s, e) => DisminuirTiempo();
        }

        // 
        public MarcadorGlobal GetMarcador() => _marcador;

        public EstadoTiempoDto GetEstadoTiempo()
        {
            var estado = _corriendo ? "Running"
                                    : (_marcador.TiempoRestante == _duracionPorCuartoSeg ? "Stopped" : "Paused");

            return new EstadoTiempoDto
            {
                Estado = estado,
                CuartoActual = _marcador.CuartoActual,
                SegundosRestantes = _marcador.TiempoRestante,
                DuracionCuarto = _duracionPorCuartoSeg
            };
        }

        // ====== Puntos ======
        public void SumarPuntos(string equipo, int puntos)
        {
            var eq = ObtenerEquipo(equipo);
            eq.Puntos += puntos;
            _context.SaveChanges();
        }

        public void RestarPuntos(string equipo, int puntos)
        {
            var eq = ObtenerEquipo(equipo);
            eq.Puntos = Math.Max(0, eq.Puntos - puntos);
            _context.SaveChanges();
        }

        // ====== Faltas ======
        public void RegistrarFalta(string equipo)
        {
            var eq = ObtenerEquipo(equipo);
            eq.Faltas += 1;
            _context.SaveChanges();
        }

        // ====== Cuartos ======
        public void AvanzarCuarto()
        {
            // Paramos el tiempo para cambiar de cuarto
            PausarTiempo();

            _marcador.CuartoActual += 1;
            _marcador.TiempoRestante = _duracionPorCuartoSeg;

            _context.SaveChanges();
        }

        // ====== Tiempo ======
        public void IniciarTiempo()
        {
            if (_corriendo) return;
            _corriendo = true;
            _timer.Start();

        }

        public void PausarTiempo()
        {
            if (!_corriendo) return;
            _corriendo = false;
            _timer.Stop();
            _context.SaveChanges();
        }

        public void ReanudarTiempo() => IniciarTiempo();

        public void ReiniciarTiempo()
        {
            _corriendo = false;
            _timer.Stop();
            _marcador.TiempoRestante = _duracionPorCuartoSeg;
            _context.SaveChanges();
        }

        public void ConfigurarDuracion(int segundos)
        {
            if (segundos < 60 || segundos > 3600)
                throw new ArgumentOutOfRangeException(nameof(segundos), "Duración entre 60 y 3600 segundos.");

            if (_corriendo)
                throw new InvalidOperationException("No puedes cambiar la duración mientras el tiempo está corriendo.");

            _duracionPorCuartoSeg = segundos;

            // Aquí decido reiniciar el tiempo del cuarto actual a la nueva duración
            _marcador.TiempoRestante = _duracionPorCuartoSeg;

            _context.SaveChanges();
        }

        // ======  temporizador ======
        private void DisminuirTiempo()
        {
            if (!_corriendo) return;

            if (_marcador.TiempoRestante > 0)
            {
                _marcador.TiempoRestante--;
            }
            else
            {
                PausarTiempo();
            }
        }

        private Equipo ObtenerEquipo(string equipo)
        {
            if (string.Equals(equipo, "Local", StringComparison.OrdinalIgnoreCase))
                return _marcador.EquipoLocal;

            if (string.Equals(equipo, "Visitante", StringComparison.OrdinalIgnoreCase))
                return _marcador.EquipoVisitante;

            throw new ArgumentException("El equipo debe ser 'Local' o 'Visitante'.");
        }
        
        //Reinicia todo
        public void ReiniciarMarcador()
        {
            PausarTiempo();
            _marcador.EquipoLocal.Puntos = 0;
            _marcador.EquipoLocal.Faltas = 0;
            _marcador.EquipoVisitante.Puntos = 0;
            _marcador.EquipoVisitante.Faltas = 0;
            _marcador.CuartoActual = 1;
            _marcador.TiempoRestante = _duracionPorCuartoSeg; // 600 por defecto
            _context.SaveChanges();
        }
    }
}
