using System.Globalization;
using System.Text;
using System.Text.Json;
using MonitorErrores.Models;

namespace MonitorErrores.Services;

public class ErrorKnowledgeService
{
    private readonly List<Solucion> _soluciones;

    public ErrorKnowledgeService(IWebHostEnvironment environment)
    {
        var ruta = Path.Combine(
            environment.ContentRootPath,
            "Knowledge",
            "soluciones.json");

        if (!File.Exists(ruta))
        {
            _soluciones = new List<Solucion>();
            return;
        }

        var json = File.ReadAllText(ruta);

        var opciones = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var datos = JsonSerializer.Deserialize<DatosSoluciones>(
            json,
            opciones);

        _soluciones = datos?.Soluciones ?? new List<Solucion>();

        Console.WriteLine(
            $"Soluciones cargadas: {_soluciones.Count}");
    }

    public Solucion? BuscarSolucion(
        string codigo,
        string servicio,
        string mensaje)
    {
        // Normalizamos los datos recibidos.
        var codigoNormalizado = NormalizarTexto(codigo);
        var servicioNormalizado = NormalizarTexto(servicio);
        var mensajeNormalizado = NormalizarTexto(mensaje);

        // Convertimos el mensaje en palabras.
        var palabras = mensajeNormalizado
            .Split(
                new[] { ' ', ',', '.', ':', ';', '-', '_', '(', ')' },
                StringSplitOptions.RemoveEmptyEntries);

        // ---------------------------------------------------------
        // 1. Buscar por código + servicio
        // ---------------------------------------------------------

        var solucionPorCodigo = _soluciones.FirstOrDefault(s =>
        {
            if (!s.Confirmada ||
                string.IsNullOrWhiteSpace(s.Fuente) ||
                string.IsNullOrWhiteSpace(s.TipoFuente) ||
                string.IsNullOrWhiteSpace(s.SolucionTexto))
            {
                return false;
            }

            var codigoSolucion =
                NormalizarTexto(s.Codigo);

            var servicioSolucion =
                NormalizarTexto(s.Servicio);

            return codigoSolucion == codigoNormalizado &&
                   servicioSolucion == servicioNormalizado;
        });

        if (solucionPorCodigo != null)
        {
            Console.WriteLine(
                "Solución encontrada por código y servicio.");

            return solucionPorCodigo;
        }

        // ---------------------------------------------------------
        // 2. Buscar por coincidencia del mensaje
        // ---------------------------------------------------------

        var solucionPorMensaje = _soluciones.FirstOrDefault(s =>
        {
            if (!s.Confirmada ||
                string.IsNullOrWhiteSpace(s.Fuente) ||
                string.IsNullOrWhiteSpace(s.TipoFuente) ||
                string.IsNullOrWhiteSpace(s.SolucionTexto))
            {
                return false;
            }

            var servicioSolucion =
                NormalizarTexto(s.Servicio);

            if (servicioSolucion != servicioNormalizado)
            {
                return false;
            }

            var problema =
                NormalizarTexto(s.Problema);

            var coincidencias = palabras.Count(p =>
                p.Length >= 4 &&
                problema.Contains(
                    p,
                    StringComparison.OrdinalIgnoreCase));

            return coincidencias >= 2;
        });

        if (solucionPorMensaje != null)
        {
            Console.WriteLine(
                "Solución encontrada por coincidencia del mensaje.");

            return solucionPorMensaje;
        }

        // ---------------------------------------------------------
        // 3. Buscar por palabras clave
        // ---------------------------------------------------------

        var solucionPorPalabraClave = _soluciones.FirstOrDefault(s =>
        {
            if (!s.Confirmada ||
                string.IsNullOrWhiteSpace(s.Fuente) ||
                string.IsNullOrWhiteSpace(s.TipoFuente) ||
                string.IsNullOrWhiteSpace(s.SolucionTexto))
            {
                return false;
            }

            var servicioSolucion =
                NormalizarTexto(s.Servicio);

            if (servicioSolucion != servicioNormalizado)
            {
                return false;
            }

            return s.PalabrasClave.Any(palabraClave =>
            {
                var palabraClaveNormalizada =
                    NormalizarTexto(palabraClave);

                return palabras.Any(palabra =>
                    palabra.Equals(
                        palabraClaveNormalizada,
                        StringComparison.OrdinalIgnoreCase));
            });
        });

        if (solucionPorPalabraClave != null)
        {
            Console.WriteLine(
                "Solución encontrada por palabra clave.");

            return solucionPorPalabraClave;
        }

        // No se encontró una solución confiable.
        return null;
    }

    // -------------------------------------------------------------
    // Normalización de texto
    // -------------------------------------------------------------

    private static string NormalizarTexto(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return "";
        }

        var textoNormalizado = texto
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        var resultado = new StringBuilder();

        foreach (var caracter in textoNormalizado)
        {
            var categoria =
                CharUnicodeInfo.GetUnicodeCategory(caracter);

            if (categoria != UnicodeCategory.NonSpacingMark)
            {
                resultado.Append(caracter);
            }
        }

        return resultado
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }

    private class DatosSoluciones
    {
        public List<Solucion> Soluciones { get; set; } = new();
    }
}