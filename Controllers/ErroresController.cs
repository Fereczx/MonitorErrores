using Microsoft.AspNetCore.Mvc;
using MonitorErrores.Models;
using MonitorErrores.Services;

namespace MonitorErrores.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ErroresController : ControllerBase
{
    private readonly ErrorService _errorService;
    private readonly IAService _iaService;

    public ErroresController(
        ErrorService errorService,
        IAService iaService)
    {
        _errorService = errorService;
        _iaService = iaService;
    }

    [HttpPost]
    public IActionResult ProcesarError([FromBody] Error error)
    {
        var resultado = _errorService.ProcesarError(error);

        return Ok(resultado);
    }
    [HttpGet]
    public IActionResult ObtenerErrores()
    {
        var errores = _errorService.ObtenerErrores();

        return Ok(errores);
    }
    [HttpGet("{codigo}")]
    public IActionResult ObtenerErroresPorCodigo(string codigo)
    {
        var errores = _errorService.ObtenerErroresPorCodigo(codigo);

        return Ok(errores);
    }
    [HttpPost("diagnosticar")]
    public IActionResult DiagnosticarError([FromBody] Error error)
    {
        var diagnostico = _errorService.DiagnosticarError(error);

        return Ok(diagnostico);
    }
    [HttpPost("ia")]
    public async Task<IActionResult> ProbarIA([FromBody] Error error)
    {
        var respuesta = await _iaService.AnalizarError(
            error.Codigo,
            error.Servicio,
            error.Mensaje,
            0,
            false);

        return Ok(new
        {
            codigo = error.Codigo,
            respuesta = respuesta
        });
    }
}