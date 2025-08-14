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
            _service.SumarPuntos(equipo, puntos);
            return Ok(_service.GetMarcador());
        }

        // POST api/marcador/puntos/restar
        [HttpPost("puntos/restar")]
        public IActionResult RestarPuntos([FromQuery] string equipo, [FromQuery] int puntos)
        {
            _service.RestarPuntos(equipo, puntos);
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
            _service.RegistrarFalta(equipo);
            return Ok(_service.GetMarcador());
        }
    }
}
