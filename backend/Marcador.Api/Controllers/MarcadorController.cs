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

        // Lecturas
        [HttpGet]
        [ProducesResponseType(typeof(MarcadorGlobal), StatusCodes.Status200OK)]
        public ActionResult<MarcadorGlobal> GetMarcador() => Ok(_service.GetMarcador());

        [HttpGet("tiempo")]
        [ProducesResponseType(typeof(EstadoTiempoDto), StatusCodes.Status200OK)]
        public ActionResult<EstadoTiempoDto> GetTiempo() => Ok(_service.GetEstadoTiempo());

        // Puntos
        [HttpPost("puntos/sumar")]
        [ProducesResponseType(typeof(MarcadorGlobal), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult SumarPuntos([FromBody] PuntosDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            _service.SumarPuntos(dto.Equipo, dto.Puntos);
            return Ok(_service.GetMarcador());
        }

        [HttpPost("puntos/restar")]
        [ProducesResponseType(typeof(MarcadorGlobal), StatusCodes.Status200OK)]
        public IActionResult RestarPuntos([FromBody] PuntosDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            _service.RestarPuntos(dto.Equipo, dto.Puntos);
            return Ok(_service.GetMarcador());
        }

        // Faltas
        [HttpPost("falta")]
        [ProducesResponseType(typeof(MarcadorGlobal), StatusCodes.Status200OK)]
        public IActionResult RegistrarFalta([FromBody] FaltaDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            _service.RegistrarFalta(dto.Equipo);
            return Ok(_service.GetMarcador());
        }

        // Tiempo
        [HttpPost("tiempo/configurar-duracion")]
        [ProducesResponseType(typeof(MarcadorGlobal), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public IActionResult ConfigurarDuracion([FromBody] ConfigTiempoDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            try
            {
                _service.ConfigurarDuracion(dto.DuracionSegundosPorCuarto);
                return Ok(_service.GetMarcador());
            }
            catch (InvalidOperationException ex)
            {
                return Problem(title: "Conflicto de estado", detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
            }
        }

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
        public IActionResult ReiniciarTiempo()
        {
            _service.ReiniciarTiempo();
            return Ok(_service.GetMarcador());
        }

        [HttpPost("cuarto/siguiente")]
        public IActionResult AvanzarCuarto()
        {
            _service.AvanzarCuarto();
            return Ok(_service.GetMarcador());
        }

        [HttpPost("reiniciar-marcador")]
        public IActionResult ReiniciarMarcador()
        {
            _service.ReiniciarMarcador();
            return Ok(_service.GetMarcador());
        }
    }

}
