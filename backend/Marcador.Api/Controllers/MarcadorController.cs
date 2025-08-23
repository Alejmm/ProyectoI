using Microsoft.AspNetCore.Mvc;
using Marcador.Api.Models;
using Marcador.Api.Services;
using Marcador.Api.Dtos;

namespace Marcador.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarcadorController : ControllerBase
    {
        private readonly MarcadorService _service;
        public MarcadorController(MarcadorService service) => _service = service;

        // ---- Lecturas ----
        [HttpGet]
        public ActionResult<MarcadorGlobal> GetMarcador() => Ok(_service.GetMarcador());

        [HttpGet("tiempo")]
        public ActionResult<EstadoTiempoDto> GetTiempo() => Ok(_service.GetEstadoTiempo());

        // ---- Puntos ----
        [HttpPost("puntos/sumar")]
        public IActionResult SumarPuntos([FromQuery] string equipo, [FromQuery] int puntos)
        {
            if (!TryNormalizarEquipo(equipo, out var eq)) return BadRequest("Parametro 'equipo' debe ser 'local' o 'visitante'.");
            _service.SumarPuntos(eq, puntos);
            return Ok(_service.GetMarcador());
        }

        [HttpPost("puntos/restar")]
        public IActionResult RestarPuntos([FromQuery] string equipo, [FromQuery] int puntos)
        {
            if (!TryNormalizarEquipo(equipo, out var eq)) return BadRequest("Parametro 'equipo' debe ser 'local' o 'visitante'.");
            _service.RestarPuntos(eq, puntos);
            return Ok(_service.GetMarcador());
        }

        // ---- Faltas ----
        [HttpPost("falta")]
        public IActionResult RegistrarFalta([FromQuery] string equipo)
        {
            if (!TryNormalizarEquipo(equipo, out var eq)) return BadRequest("Parametro 'equipo' debe ser 'local' o 'visitante'.");
            _service.RegistrarFalta(eq);
            return Ok(_service.GetMarcador());
        }

        // ---- Cuartos ----
        [HttpPost("cuarto/siguiente")]
        public IActionResult AvanzarCuarto()
        {
            _service.AvanzarCuarto();
            return Ok(_service.GetMarcador());
        }

        // ---- Tiempo (rutas existentes) ----
        [HttpPost("tiempo/iniciar")]
        public IActionResult IniciarTiempo()
        {
            _service.IniciarTiempo();
            return Ok(_service.GetMarcador());
        }

        [HttpPost("tiempo/pausar")]
        public IActionResult PausarTiempo()
        {
            _service.PausarTiempo();
            return Ok(_service.GetMarcador());
        }

        [HttpPost("tiempo/reanudar")]
        public IActionResult ReanudarTiempo()
        {
            _service.ReanudarTiempo();
            return Ok(_service.GetMarcador());
        }

        [HttpPost("tiempo/reiniciar")]
        public ActionResult<MarcadorGlobal> ReiniciarTiempo([FromQuery] int? seg)
        {
            var resultado = _service.ReiniciarTiempo(seg ?? 600);
            return Ok(resultado);
        }

        [HttpPost("tiempo/establecer")]
        public ActionResult<MarcadorGlobal> EstablecerTiempo([FromQuery] int seg)
        {
            var resultado = _service.EstablecerTiempo(seg);
            return Ok(resultado);
        }

        // ---- Tiempo  ----
        [HttpPost("reloj/iniciar")]
        public ActionResult<MarcadorGlobal> IniciarReloj()
        {
            _service.IniciarReloj();
            return Ok(_service.GetMarcador());
        }

        [HttpPost("reloj/pausar")]
        public ActionResult<MarcadorGlobal> PausarReloj()
        {
            _service.PausarReloj();
            return Ok(_service.GetMarcador());
        }

        // ---- Equipos  ----
        public record RenombrarEquiposDto(string? Local, string? Visitante);

        [HttpPost("equipos/renombrar")]
        public ActionResult<MarcadorGlobal> RenombrarEquipos([FromBody] RenombrarEquiposDto dto)
        {
            var res = _service.RenombrarEquipos(dto.Local, dto.Visitante);
            return Ok(res);
        }

        [HttpPost("nuevo")]
        public ActionResult<MarcadorGlobal> Nuevo() => Ok(_service.NuevoPartido());
       
        private static bool TryNormalizarEquipo(string? equipo, out string normalizado)
        {
            normalizado = "";
            if (string.IsNullOrWhiteSpace(equipo)) return false;
            var e = equipo.Trim().ToLowerInvariant();
            if (e == "local") { normalizado = "Local"; return true; }
            if (e == "visitante") { normalizado = "Visitante"; return true; }
            return false;
        }
    }
}
