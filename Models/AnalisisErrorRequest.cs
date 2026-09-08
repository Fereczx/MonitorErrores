namespace MonitorErrores.Models;

public class AnalisisErrorRequest
{
    public string? Codigo { get; set; }

    public string? Servicio { get; set; }

    public string? Mensaje { get; set; }

    public IFormFile? Imagen { get; set; }
}