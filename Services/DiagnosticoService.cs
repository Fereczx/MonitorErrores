using MonitorErrores.Models;

namespace MonitorErrores.Services;

public class DiagnosticoService
{
    private readonly ErrorService _errorService;
    private readonly IIAService _iaService;

    public DiagnosticoService(
        ErrorService errorService,
        IIAService iaService)
    {
        _errorService = errorService;
        _iaService = iaService;
    }

    public async Task<object> Diagnosticar(Error error)
    {
        Console.WriteLine(
            $"Buscando solución para: {error.Codigo} / {error.Servicio}");

        // 1. Buscar primero en nuestra base de conocimiento
        var solucion = _errorService.BuscarSolucion(error);

        if (solucion != null)
        {
            Console.WriteLine(
                "Solución encontrada en la base de conocimiento.");

            return new
            {
                origen = "conocimiento",
                codigo = error.Codigo,
                servicio = error.Servicio,
                problema = solucion.Problema,
                solucion = solucion.SolucionTexto,
                mensajeUsuario = solucion.MensajeUsuario,
                fuente = solucion.Fuente,
                tipoFuente = solucion.TipoFuente,
                confirmada = solucion.Confirmada
            };
        }

        // 2. Si la IA no está disponible,
        // no intentamos consultar la IA nuevamente.
        if (error.Codigo == "IA_NO_DISPONIBLE")
        {
            Console.WriteLine(
                "La IA no está disponible. No se realizará otra consulta.");

            return new
            {
                origen = "sistema",
                codigo = error.Codigo,
                servicio = error.Servicio,
                problema = error.Mensaje,
                solucion = "El servicio de diagnóstico mediante IA no está disponible actualmente. Intente nuevamente más tarde.",
                fuente = "Sistema de diagnóstico",
                confirmada = false
            };
        }

        // 3. Si no conocemos el error, consultar a la IA
        var respuestaIA = await _iaService.AnalizarError(
            error.Codigo,
            error.Servicio,
            error.Mensaje);

        if (respuestaIA == null || !respuestaIA.SolucionCompleta)
        {
            Console.WriteLine(
                "La IA no encontró una solución completa y confiable.");

            return new
            {
                origen = "ia",
                codigo = error.Codigo,
                servicio = error.Servicio,
                problema = respuestaIA?.Problema ?? error.Mensaje,
                solucion = "",
                mensajeUsuario =
                    "No pudimos determinar una solución para este problema. Por favor, comunicate con Atención al Cliente para recibir asistencia.",
                fuente = respuestaIA?.Fuente ?? "",
                tipoFuente = respuestaIA?.TipoFuente ?? "",
                solucionCompleta = false
            };
        }

        Console.WriteLine(
            "La IA encontró una solución completa y confiable.");

        return new
        {
            origen = "ia",
            codigo = error.Codigo,
            servicio = error.Servicio,
            problema = respuestaIA.Problema,
            solucion = respuestaIA.Solucion,
            mensajeUsuario = respuestaIA.MensajeUsuario,
            fuente = respuestaIA.Fuente,
            tipoFuente = respuestaIA.TipoFuente,
            solucionCompleta = true
        };
    }
}