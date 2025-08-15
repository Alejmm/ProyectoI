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

        // ---- Puntos (vía query para hacerlo simple) ----
        // POST /api/marcador/puntos/sumar?equipo=Local&puntos=2
        [HttpPost("puntos/sumar")]
        public IActionResult SumarPuntos([FromQuery] string equipo, [FromQuery] int puntos)
        {
            if (!TryNormalizarEquipo(equipo, out var eq)) return BadRequest("Parametro 'equipo' debe ser 'local' o 'visitante'.");
            _service.SumarPuntos(eq, puntos);
            return Ok(_service.GetMarcador());
        }

        // POST /api/marcador/puntos/restar?equipo=Visitante&puntos=1
        [HttpPost("puntos/restar")]
        public IActionResult RestarPuntos([FromQuery] string equipo, [FromQuery] int puntos)
        {
            if (!TryNormalizarEquipo(equipo, out var eq)) return BadRequest("Parametro 'equipo' debe ser 'local' o 'visitante'.");
            _service.RestarPuntos(eq, puntos);
            return Ok(_service.GetMarcador());
        }

        // ---- Faltas ----
        // POST /api/marcador/falta?equipo=Local
        [HttpPost("falta")]
        public IActionResult RegistrarFalta([FromQuery] string equipo)
        {
            if (!TryNormalizarEquipo(equipo, out var eq)) return BadRequest("Parametro 'equipo' debe ser 'local' o 'visitante'.");
            _service.RegistrarFalta(eq);
            return Ok(_service.GetMarcador());
        }

        // ---- Tiempo ----
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

        // POST /api/marcador/tiempo/reiniciar?seg=600
        [HttpPost("tiempo/reiniciar")]
        public ActionResult<MarcadorGlobal> ReiniciarTiempo([FromQuery] int? seg)
        {
            var resultado = _service.ReiniciarTiempo(seg ?? 600);
            return Ok(resultado);
        }

        // POST /api/marcador/tiempo/establecer?seg=545
        [HttpPost("tiempo/establecer")]
        public ActionResult<MarcadorGlobal> EstablecerTiempo([FromQuery] int seg)
        {
            var resultado = _service.EstablecerTiempo(seg);
            return Ok(resultado);
        }

        // ---- Cuartos ----
        [HttpPost("cuarto/siguiente")]
        public IActionResult AvanzarCuarto()
        {
            _service.AvanzarCuarto();
            return Ok(_service.GetMarcador());
        }

        // helper
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
