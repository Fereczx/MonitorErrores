using Microsoft.AspNetCore.Mvc;
using MonitorErrores.Models;
using MonitorErrores.Services;

namespace MonitorErrores.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ErroresController : ControllerBase
{
    private readonly DiagnosticoService _diagnosticoService;
    private readonly IAService _iaService;

    public ErroresController(
        DiagnosticoService diagnosticoService,
        IAService iaService)
    {
        _diagnosticoService = diagnosticoService;
        _iaService = iaService;
    }

    [HttpPost]
    public async Task<IActionResult> Diagnosticar(
        [FromBody] Error error)
    {
        var resultado =
            await _diagnosticoService.Diagnosticar(error);

        return Ok(resultado);
    }

    [HttpPost("imagen")]
    public async Task<IActionResult> AnalizarImagen(
        [FromForm] AnalisisErrorRequest request)
    {
        Console.WriteLine(
            "=== ENDPOINT IMAGEN RECIBIDO ===");

        if (request.Imagen == null)
        {
            Console.WriteLine(
                "NO SE RECIBIÓ LA IMAGEN.");

            return BadRequest(new
            {
                mensaje = "No se recibió ninguna imagen."
            });
        }

        Console.WriteLine(
            $"Imagen recibida: {request.Imagen.FileName}");

        Console.WriteLine(
            $"Tipo: {request.Imagen.ContentType}");

        Console.WriteLine(
            $"Tamaño: {request.Imagen.Length} bytes.");

        Console.WriteLine(
            "Llamando a IAService.AnalizarImagen...");

        var diagnostico =
            await _iaService.AnalizarImagen(
                request.Imagen.OpenReadStream(),
                request.Imagen.FileName);

        Console.WriteLine(
            "IAService terminó.");

        if (diagnostico == null)
        {
            Console.WriteLine(
                "IAService devolvió NULL.");

            return StatusCode(503, new
            {
                mensaje = "No fue posible analizar la imagen."
            });
        }

        Console.WriteLine(
            $"Código detectado: {diagnostico.Codigo}");

        Console.WriteLine(
            $"Servicio detectado: {diagnostico.Servicio}");

        Console.WriteLine(
            $"Problema detectado: {diagnostico.Problema}");

        var error = new Error
        {
            Codigo = diagnostico.Codigo,
            Servicio = diagnostico.Servicio,
            Mensaje = diagnostico.Problema
        };

        var resultado =
            await _diagnosticoService.Diagnosticar(error);

        return Ok(resultado);
    }
}