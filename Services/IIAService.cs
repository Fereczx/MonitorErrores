using MonitorErrores.Models;

namespace MonitorErrores.Services;

public interface IIAService
{
    Task<ResultadoIA?> AnalizarError(
        string codigo,
        string servicio,
        string mensaje);

    Task<DiagnosticoIA?> AnalizarImagen(
        Stream imagen,
        string nombreArchivo);
}