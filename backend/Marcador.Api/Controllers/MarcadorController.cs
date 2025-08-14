using Microsoft.AspNetCore.Mvc;
using Marcador.Api.Models;
using Marcador.Api.Services;

namespace Marcador.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarcadorController : ControllerBase
    {
        private readonly MarcadorService _service;

        public MarcadorController(MarcadorService service)
        {
            _service = service;
        }

        // GET api/marcador
        [HttpGet]
        public ActionResult<MarcadorGlobal> GetMarcador()
        {
            return Ok(_service.GetMarcador());
        }

        // POST api/marcador/puntos/sumar
        [HttpPost("puntos/sumar")]
        public IActionResult SumarPuntos([FromQuery] string equipo, [FromQuery] int puntos)
        {
    if (!TryNormalizarEquipo(equipo, out var eqNormalizado))
        return BadRequest("Parametro 'equipo' debe ser 'local' o 'visitante'.");

    _service.SumarPuntos(eqNormalizado, puntos);
    return Ok(_service.GetMarcador());
        }

        // POST api/marcador/puntos/restar
        [HttpPost("puntos/restar")]
        public IActionResult RestarPuntos([FromQuery] string equipo, [FromQuery] int puntos)
        {
    if (!TryNormalizarEquipo(equipo, out var eqNormalizado))
        return BadRequest("Parametro 'equipo' debe ser 'local' o 'visitante'.");

    _service.RestarPuntos(eqNormalizado, puntos);
    return Ok(_service.GetMarcador());
        }

        // POST api/marcador/cuarto/siguiente
        [HttpPost("cuarto/siguiente")]
        public IActionResult AvanzarCuarto()
        {
            _service.AvanzarCuarto();
            return Ok(_service.GetMarcador());
        }

        // POST api/marcador/falta
        [HttpPost("falta")]
        public IActionResult RegistrarFalta([FromQuery] string equipo)
        {
    if (!TryNormalizarEquipo(equipo, out var eqNormalizado))
        return BadRequest("Parametro 'equipo' debe ser 'local' o 'visitante'.");

    _service.RegistrarFalta(eqNormalizado);
    return Ok(_service.GetMarcador());
        }

        // Helper
        private static bool TryNormalizarEquipo(string? equipo, out string normalizado)
{
    normalizado = "";
    if (string.IsNullOrWhiteSpace(equipo)) return false;
    var e = equipo.Trim().ToLowerInvariant();
    if (e == "local") { normalizado = "Local"; return true; }
    if (e == "visitante") { normalizado = "Visitante"; return true; }
    return false;
}
// POST /api/marcador/tiempo/establecer?seg=545
[HttpPost("tiempo/establecer")]
public ActionResult<MarcadorGlobal> EstablecerTiempo([FromQuery] int seg)
{
    var resultado = _service.EstablecerTiempo(seg);
    return Ok(resultado);
}

// POST /api/marcador/tiempo/reiniciar   
[HttpPost("tiempo/reiniciar")]
public ActionResult<MarcadorGlobal> ReiniciarTiempo([FromQuery] int? seg)
{
    var resultado = _service.ReiniciarTiempo(seg ?? 600);
    return Ok(resultado);
}
    }
}
