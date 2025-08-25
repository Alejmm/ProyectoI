using System;
using Microsoft.EntityFrameworkCore;
using Marcador.Api.Models;
using Marcador.Api.Dtos;

namespace Marcador.Api.Services
{
    public class MarcadorService
    {
        private const int DURACION_CUARTO_DEF   = 600; // 10:00 por defecto
        private const int DURACION_PRORROGA_DEF = 300; //  5:00 por defecto

        private readonly MarcadorDbContext _context;
        private static readonly object _lock = new();

        // Estado del reloj en memoria (fuente de verdad temporal)
        private static bool _corriendo = false;
        private static  DateTime _inicioUtc;          // marca de tiempo del último iniciar/reanudar
        
        // Alias de compatibilidad: duración vigente del periodo actual
        private int _duracionPorCuarto =>
        _marcador?.EnProrroga == true ? _duracionProrrogaSeg : _duracionActualSeg;

        // Duraciones actuales (configurables)
        private static int _duracionActualSeg = DURACION_CUARTO_DEF;   // lo que se aplica desde el front (cuarto normal)
        private static int _duracionProrrogaSeg = DURACION_PRORROGA_DEF; // por si quieres hacerlo configurable luego

        // Entidad persistida base
        private static MarcadorGlobal? _marcador;

        public MarcadorService(MarcadorDbContext context)
        {
            _context = context;

            lock (_lock)
            {
                if (_marcador == null)
                {
                    _marcador = _context.Marcadores
                        .Include(m => m.EquipoLocal)
                        .Include(m => m.EquipoVisitante)
                        .FirstOrDefault() ?? new MarcadorGlobal
                        {
                            EquipoLocal = new Equipo { Nombre = "Local", Puntos = 0, Faltas = 0, Jugadores = new() },
                            EquipoVisitante = new Equipo { Nombre = "Visitante", Puntos = 0, Faltas = 0, Jugadores = new() },
                            CuartoActual = 1,
                            TiempoRestante = DURACION_CUARTO_DEF,
                            EnProrroga = false,
                            NumeroProrroga = 0,
                            RelojCorriendo = false
                        };

                    if (_marcador.Id == 0)
                    {
                        _context.Marcadores.Add(_marcador);
                        _context.SaveChanges();
                    }

                    // saneos
                    _marcador.EquipoLocal     ??= new Equipo { Nombre = "Local", Puntos = 0, Faltas = 0, Jugadores = new() };
                    _marcador.EquipoVisitante ??= new Equipo { Nombre = "Visitante", Puntos = 0, Faltas = 0, Jugadores = new() };
                    if (_marcador.CuartoActual <= 0) _marcador.CuartoActual = 1;
                    if (_marcador.TiempoRestante < 0) _marcador.TiempoRestante = 0;

                    // inicializa duración actual
                    _duracionActualSeg = Math.Max(1, _marcador.TiempoRestante == 0 ? DURACION_CUARTO_DEF : _marcador.TiempoRestante);
                }
            }
        }

        // Tiempo
        // Calcula el tiempo “ahora” 
        private int TiempoActual()
        {
            if (!_corriendo) return Math.Max(0, _marcador!.TiempoRestante);

            var transcurrido = (int)Math.Floor((DateTime.UtcNow - _inicioUtc).TotalSeconds);
            var restante = Math.Max(0, _marcador!.TiempoRestante - transcurrido);
            // Apaga automáticamente el reloj si llegó a 0
            if (restante <= 0)
            {
                _corriendo = false;
                _marcador.RelojCorriendo = false;
                restante = 0;
            }

            return restante;
        }

        
        private MarcadorGlobal Proyeccion()
        {
            return new MarcadorGlobal
            {
                Id = _marcador.Id,
                EquipoLocalId = _marcador.EquipoLocalId,
                EquipoVisitanteId = _marcador.EquipoVisitanteId,
                EquipoLocal = _marcador.EquipoLocal,
                EquipoVisitante = _marcador.EquipoVisitante,
                CuartoActual = _marcador.CuartoActual,
                EnProrroga = _marcador.EnProrroga,
                NumeroProrroga = _marcador.NumeroProrroga,
                TiempoRestante = TiempoActual(),
                RelojCorriendo = _corriendo
            };
        }

        private Equipo ObtenerEquipo(string equipo)
        {
            if (string.Equals(equipo, "local", StringComparison.OrdinalIgnoreCase)) return _marcador.EquipoLocal;
            if (string.Equals(equipo, "visitante", StringComparison.OrdinalIgnoreCase)) return _marcador.EquipoVisitante;
            // compat con llamadas antiguas
            if (string.Equals(equipo, "Local", StringComparison.OrdinalIgnoreCase)) return _marcador.EquipoLocal;
            if (string.Equals(equipo, "Visitante", StringComparison.OrdinalIgnoreCase)) return _marcador.EquipoVisitante;

            throw new ArgumentException("El equipo debe ser 'local' o 'visitante'.");
        }

        // ---------- Lecturas ----------
        public MarcadorGlobal GetMarcador()
        {
            lock (_lock) return Proyeccion();
        }

        public EstadoTiempoDto GetEstadoTiempo()
        {
            lock (_lock)
            {
                var dur = _marcador.EnProrroga ? _duracionProrrogaSeg : _duracionActualSeg;
                var seg = TiempoActual();
                var estado = _corriendo ? "Running" : (seg == dur ? "Stopped" : "Paused");

                return new EstadoTiempoDto
                {
                    Estado = estado,
                    CuartoActual = _marcador.CuartoActual,
                    SegundosRestantes = seg,
                    DuracionCuarto = dur
                };
            }
        }

        // ---------- Puntos ----------
        public void SumarPuntos(string equipo, int puntos)
        {
            lock (_lock)
            {
                var eq = ObtenerEquipo(equipo);
                eq.Puntos += Math.Max(0, puntos);
                _context.SaveChanges();
            }
        }

        public void RestarPuntos(string equipo, int puntos)
        {
            lock (_lock)
            {
                var eq = ObtenerEquipo(equipo);
                eq.Puntos = Math.Max(0, eq.Puntos - Math.Max(0, puntos));
                _context.SaveChanges();
            }
        }

        // ---------- Faltas ----------
        public void RegistrarFalta(string equipo)
        {
            lock (_lock)
            {
                var eq = ObtenerEquipo(equipo);
                eq.Faltas += 1;
                _context.SaveChanges();
            }
        }

        // ---------- Cuartos ----------
        //Este metodo guarda cuando avanzaNormal el marcador y termina a los 4 cuartos
        public void AvanzarCuarto()
        {
            lock (_lock)
            {
                // Detener y materializar antes de cambiar periodo
                if (_corriendo)
                {
                    _marcador.TiempoRestante = TiempoActual();
                    _corriendo = false;
                }
                _marcador.RelojCorriendo = false;

                // Si estaba en el 4.º cuarto (y no venía de prórroga), cerramos y guardamos
                if (_marcador.CuartoActual >= 4 && !_marcador.EnProrroga)
                {
                    GuardarCierrePartido("TerminadoAuto", "Tiempofinalizado");
                    _context.SaveChanges();
                    return;
                }

                if (_marcador.CuartoActual < 4)
                {
                    _marcador.CuartoActual++;
                    _marcador.EnProrroga = false;
                    _marcador.NumeroProrroga = 0;
                    _marcador.TiempoRestante = _duracionActualSeg;
                }
                else
                {
                    _marcador.EnProrroga = true;
                    _marcador.NumeroProrroga++;
                    _marcador.TiempoRestante = _duracionProrrogaSeg;
                }

                // Reinicio de faltas por periodo
                _marcador.EquipoLocal.Faltas = 0;
                _marcador.EquipoVisitante.Faltas = 0;

                _context.SaveChanges();
            }
        }

        //Siguiente cuarto avanzaManual
        public MarcadorGlobal SiguienteCuarto()
        {
            // reloj a cero → ya debiste pausar y materializar tiempo   
            if (_marcador.CuartoActual < 4 || _marcador.EnProrroga)
            {
                _marcador.CuartoActual++;
                _marcador.TiempoRestante = _duracionActualSeg; // resetea para el nuevo cuarto
                return Proyeccion();
            }

            // Estaba en cuarto 4 y llegó a 0 ⇒ cierre automático
            GuardarCierrePartido("TerminadoAuto", "Tiempo finalizado");
            return Proyeccion();
        }
        // ---------- Tiempo ----------
        public void IniciarReloj()
        {
            lock (_lock)
            {
                if (_corriendo) return;

                _marcador!.TiempoRestante = TiempoActual();
                _inicioUtc = DateTime.UtcNow;
                _corriendo = true;
                _marcador.RelojCorriendo = true;
                // No guardamos cada segundo; persistimos en pausa/avances/etc.
            }
        }

        public void PausarReloj()
        {
            lock (_lock)
            {
                if (!_corriendo) return;

                _marcador.TiempoRestante = TiempoActual(); // materializa
                _corriendo = false;
                _marcador.RelojCorriendo = false;
                _context.SaveChanges();
            }
        }

        public MarcadorGlobal EstablecerTiempo(int segundos)
        {
            lock (_lock)
            {
                segundos = Math.Max(0, segundos);
                _marcador.TiempoRestante = segundos;

                // si está corriendo, reiniciamos marca de inicio para calcular desde nuevo baseline
                if (_corriendo) _inicioUtc = DateTime.UtcNow;

                _context.SaveChanges();
                return Proyeccion();
            }
        }

        // Reinicia el periodo 
        public MarcadorGlobal ReiniciarTiempo(int segundos = DURACION_CUARTO_DEF)
        {
            lock (_lock)
            {
                segundos = Math.Max(0, segundos);

                // Actualizamos la duración CONFIGURADA del cuarto normal
                _duracionActualSeg = segundos;

                // El reinicio SIEMPRE deja pausado en el nuevo valor
                _corriendo = false;
                _marcador.RelojCorriendo = false;
                _marcador.EnProrroga = false;       // por si venías de prórroga y quieres reiniciar cuarto
                _marcador.NumeroProrroga = 0;
                _marcador.TiempoRestante = segundos;

                _context.SaveChanges();
                return Proyeccion();
            }
        }

        public MarcadorGlobal RenombrarEquipos(string? nombreLocal, string? nombreVisitante)
        {
            lock (_lock)
            {
                if (!string.IsNullOrWhiteSpace(nombreLocal))
                    _marcador.EquipoLocal.Nombre = nombreLocal.Trim();
                if (!string.IsNullOrWhiteSpace(nombreVisitante))
                    _marcador.EquipoVisitante.Nombre = nombreVisitante.Trim();

                _context.SaveChanges();
                return Proyeccion();
            }
        }
        
        public MarcadorGlobal NuevoPartido()
        {
            lock (_lock)
            {
                // Detener
                _corriendo = false;
                _marcador!.RelojCorriendo = false;
                
                // Reiniciar estado de periodo
                _marcador.CuartoActual = 1;
                _marcador.EnProrroga = false;
                _marcador.NumeroProrroga = 0;

                // Reiniciar puntos/faltas
                _marcador.EquipoLocal.Puntos = 0;
                _marcador.EquipoLocal.Faltas = 0;
                _marcador.EquipoVisitante.Puntos = 0;
                _marcador.EquipoVisitante.Faltas = 0;

                // Usa la duración CONFIGURADA del cuarto normal
                _marcador.TiempoRestante = Math.Max(1, _duracionActualSeg);

                _context.SaveChanges();
                return Proyeccion();
            }
        }
        // Para cuado termina el tiempo
        public MarcadorGlobal TerminarPartido(string? motivo)
        {
            GuardarCierrePartido("Terminado", motivo);
            return Proyeccion(); // devuelve el estado al front con el reloj detenido
        }

        private PartidoHistorico GuardarHistorico(string estado, string? motivo)
        {
            // _marcador y _duracionActualSeg ya están en tu servicio
            var h = new PartidoHistorico
            {
                Estado = estado,
                EquipoLocalId = _marcador.EquipoLocalId,
                EquipoVisitanteId = _marcador.EquipoVisitanteId,
                NombreLocal = _marcador.EquipoLocal.Nombre,
                NombreVisitante = _marcador.EquipoVisitante.Nombre,

                PuntosLocal = _marcador.EquipoLocal.Puntos,
                PuntosVisitante = _marcador.EquipoVisitante.Puntos,
                FaltasLocal = _marcador.EquipoLocal.Faltas,
                FaltasVisitante = _marcador.EquipoVisitante.Faltas,

                Cuarto = _marcador.CuartoActual,
                EnProrroga = _marcador.EnProrroga,
                NumeroProrroga = _marcador.NumeroProrroga,

                DuracionCuartoSeg = _duracionActualSeg,
                TiempoFinalSeg = TiempoActual(),

                MotivoFin = motivo
            };

            using var tx = _context.Database.BeginTransaction();
            _context.PartidosHistoricos.Add(h);
            _context.SaveChanges();
            tx.Commit();
            return h;
        }

        public enum EstadoPartido { Terminado = 1, Suspendido = 2, Cancelado = 3 }

        public MarcadorGlobal TerminarPartido(EstadoPartido estado, string? motivo)
        {
            lock (_lock)
            {
                // 1) Materializa tiempo y detiene
                if (_corriendo)
                {
                    _marcador.TiempoRestante = TiempoActual();
                    _corriendo = false;
                }
                _marcador.RelojCorriendo = false;

                // 2) Construye el histórico
                var hist = new PartidoHistorico
                {
                    Fecha = DateTime.Now,

                    EquipoLocalId   = _marcador.EquipoLocalId,
                    NombreLocal     = _marcador.EquipoLocal.Nombre,
                    PuntosLocal     = _marcador.EquipoLocal.Puntos,
                    FaltasLocal     = _marcador.EquipoLocal.Faltas,

                    EquipoVisitanteId = _marcador.EquipoVisitanteId,
                    NombreVisitante   = _marcador.EquipoVisitante.Nombre,
                    PuntosVisitante   = _marcador.EquipoVisitante.Puntos,
                    FaltasVisitante   = _marcador.EquipoVisitante.Faltas,

                    Cuarto         = _marcador.CuartoActual,
                    EnProrroga     = _marcador.EnProrroga,
                    NumeroProrroga = _marcador.NumeroProrroga,

                    DuracionCuartoSeg = _duracionActualSeg,     
                    TiempoFinalSeg     = Math.Max(0, _marcador.TiempoRestante),

                    Estado    = estado.ToString(),
                    MotivoFin = string.IsNullOrWhiteSpace(motivo) ? null : motivo.Trim()
                };

                _context.PartidosHistoricos.Add(hist);
                _context.SaveChanges();

                // Devuelve el estado actual (ya detenido)
                return Proyeccion();
            }
        }
       private PartidoHistorico GuardarCierrePartido(string estado, string? motivo)
        {
            if (_corriendo)
            {
                _marcador.TiempoRestante = TiempoActual();
                _corriendo = false;
            }
            _marcador.RelojCorriendo = false;

            var hist = new PartidoHistorico
            {
                EquipoLocalId     = _marcador.EquipoLocalId,
                EquipoVisitanteId = _marcador.EquipoVisitanteId,

                NombreLocal       = _marcador.EquipoLocal?.Nombre ?? "Local",
                PuntosLocal       = _marcador.EquipoLocal?.Puntos ?? 0,
                FaltasLocal       = _marcador.EquipoLocal?.Faltas ?? 0,

                NombreVisitante   = _marcador.EquipoVisitante?.Nombre ?? "Visitante",
                PuntosVisitante   = _marcador.EquipoVisitante?.Puntos ?? 0,
                FaltasVisitante   = _marcador.EquipoVisitante?.Faltas ?? 0,

                Estado            = estado,
                MotivoFin         = string.IsNullOrWhiteSpace(motivo) ? null : motivo.Trim(),

                Cuarto            = _marcador.CuartoActual,
                EnProrroga        = _marcador.EnProrroga,
                NumeroProrroga    = _marcador.NumeroProrroga,

                DuracionCuartoSeg = _duracionActualSeg,
                TiempoFinalSeg    = _marcador.TiempoRestante,
                Fecha             = DateTime.Now,
            };

            using var tx = _context.Database.BeginTransaction();
            _context.PartidosHistoricos.Add(hist);

            var eLocal = _context.Equipos.FirstOrDefault(e => e.Id == _marcador.EquipoLocalId);
            var eVis   = _context.Equipos.FirstOrDefault(e => e.Id == _marcador.EquipoVisitanteId);
            if (eLocal != null) { eLocal.Puntos = hist.PuntosLocal; eLocal.Faltas = hist.FaltasLocal; }
            if (eVis   != null) { eVis.Puntos   = hist.PuntosVisitante; eVis.Faltas = hist.FaltasVisitante; }

            _context.SaveChanges();
            tx.Commit();
            return hist;
        }


        public MarcadorGlobal RenombrarCreandoNuevaFicha(string? nombreLocal, string? nombreVisitante)
        {
            lock (_lock)
            {
                if (!string.IsNullOrWhiteSpace(nombreLocal))
                {
                    var nuevo = new Equipo { Nombre = nombreLocal.Trim(), Puntos = _marcador.EquipoLocal.Puntos, Faltas = _marcador.EquipoLocal.Faltas, Jugadores = new() };
                    _context.Equipos.Add(nuevo);
                    _context.SaveChanges();
                    _marcador.EquipoLocalId = nuevo.Id;
                    _marcador.EquipoLocal = nuevo;
                }

                if (!string.IsNullOrWhiteSpace(nombreVisitante))
                {
                    var nuevo = new Equipo { Nombre = nombreVisitante.Trim(), Puntos = _marcador.EquipoVisitante.Puntos, Faltas = _marcador.EquipoVisitante.Faltas, Jugadores = new() };
                    _context.Equipos.Add(nuevo);
                    _context.SaveChanges();
                    _marcador.EquipoVisitanteId = nuevo.Id;
                    _marcador.EquipoVisitante = nuevo;
                }

                _context.SaveChanges();
                return Proyeccion();
            }
        }

        public MarcadorGlobal InicializarEnCero()
        {
            lock (_lock)
            {
                if (_corriendo)
                {
                    _marcador.TiempoRestante = TiempoActual();
                    _corriendo = false;
                }

                _marcador.RelojCorriendo = false;

                _marcador.EquipoLocal.Puntos = 0;
                _marcador.EquipoLocal.Faltas = 0;
                _marcador.EquipoVisitante.Puntos = 0;
                _marcador.EquipoVisitante.Faltas = 0;

                _marcador.CuartoActual = 1;
                _marcador.EnProrroga = false;
                _marcador.NumeroProrroga = 0;

                // vuelve a la duración configurada
                _marcador.TiempoRestante = _duracionActualSeg;

                _context.SaveChanges();
                return Proyeccion();
            }
        }

        // Compat si tienes rutas viejas
        public void IniciarTiempo() => IniciarReloj();
        public void PausarTiempo()   => PausarReloj();
        public void ReanudarTiempo() => IniciarReloj();
    }
}
