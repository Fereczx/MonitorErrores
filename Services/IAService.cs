#pragma warning disable OPENAI001

using System.Text.Json;
using MonitorErrores.Models;
using OpenAI.Responses;

namespace MonitorErrores.Services;

public class IAService : IIAService
{
    private readonly ResponsesClient _responsesClient;

    public IAService(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"];

        _responsesClient = new ResponsesClient(apiKey);
    }

    public async Task<ResultadoIA?> AnalizarError(
        string codigo,
        string servicio,
        string mensaje)
    {
        var prompt = $"""
        Eres un asistente especializado en diagnosticar errores
        de aplicaciones de pagos.

        Analiza el siguiente error:

        Código: {codigo}
        Servicio: {servicio}
        Mensaje: {mensaje}

        Tu objetivo es determinar qué está ocurriendo y encontrar
        una solución confiable.

        Si es necesario, utiliza la búsqueda web para investigar.

        Reglas importantes:

        - Prioriza documentación oficial.
        - Prioriza documentación del banco, proveedor de pagos,
        API o servicio involucrado.
        - También puedes utilizar documentación técnica confiable.
        - No inventes información.
        - No inventes soluciones.
        - No presentes una solución parcial como una solución completa.
        - Si las fuentes se contradicen, considera que la solución
        NO es confiable.
        - Si no encuentras información suficiente para determinar
        una solución completa y confiable, indica que la solución
        NO está completa.

        La solución solamente puede considerarse completa cuando
        existe información suficiente para explicar el problema y
        determinar qué debe hacerse para solucionarlo.

        Responde ÚNICAMENTE con JSON válido.

        El JSON debe tener exactamente estos campos:

        problema
        solucion
        mensajeUsuario
        fuente
        tipoFuente
        solucionCompleta

        Si no existe una solución completa y confiable:

        - "solucion" debe ser una cadena vacía.
        - "mensajeUsuario" debe ser una cadena vacía.
        - "fuente" puede ser una cadena vacía.
        - "tipoFuente" puede ser una cadena vacía.
        - "solucionCompleta" debe ser false.

        No agregues texto antes ni después del JSON.
        """;

        try
        {
            Console.WriteLine(
                "Consultando IA para analizar el error...");

            var options = new CreateResponseOptions
            {
                Model = "gpt-5.6-luna"
            };

            options.Tools.Add(
                ResponseTool.CreateWebSearchTool());

            options.InputItems.Add(
                ResponseItem.CreateUserMessageItem(prompt));

            var response =
                await _responsesClient.CreateResponseAsync(options);

            var texto =
                response.Value.GetOutputText();

            Console.WriteLine(
                $"Respuesta de IA: {texto}");

            var resultado =
                JsonSerializer.Deserialize<ResultadoIA>(
                    texto,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return resultado;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"ERROR IA TEXTO: {ex}");

            return new ResultadoIA
            {
                SolucionCompleta = false
            };
        }
    }

    public async Task<DiagnosticoIA?> AnalizarImagen(
        Stream imagen,
        string nombreArchivo)
    {
        try
        {
            Console.WriteLine(
                $"Analizando imagen: {nombreArchivo}");

            var imageBytes = await ReadStreamAsync(imagen);

            Console.WriteLine(
                $"Imagen leída: {imageBytes.Length} bytes.");

            var base64Image =
                Convert.ToBase64String(imageBytes);

            var imageUri = new Uri(
                $"data:image/png;base64,{base64Image}");

            var imagePart =
                ResponseContentPart.CreateInputImagePart(
                    imageUri);

            var input = new[]
            {
                ResponseItem.CreateUserMessageItem(
                    new[]
                    {
                        ResponseContentPart.CreateInputTextPart(
                            """
                            Analiza esta captura de pantalla de un error
                            de una aplicación de pagos.

                            Identifica la información que pueda verse
                            claramente en la imagen.

                            Debes responder ÚNICAMENTE con JSON válido.

                            Utiliza exactamente esta estructura:

                            {
                              "codigo": "código del error",
                              "servicio": "servicio involucrado",
                              "problema": "explicación breve del problema"
                            }

                            Si un dato no puede determinarse,
                            utiliza una cadena vacía.

                            No inventes información.
                            """),

                        imagePart
                    })
            };

            Console.WriteLine(
                "Enviando imagen a OpenAI...");

            var response =
                await _responsesClient.CreateResponseAsync(
                    "gpt-5.6-luna",
                    input);

            Console.WriteLine(
                "Respuesta recibida desde OpenAI.");

            var texto =
                response.Value.GetOutputText();

            Console.WriteLine(
                $"Respuesta de IA para imagen: {texto}");

            var diagnostico =
                JsonSerializer.Deserialize<DiagnosticoIA>(
                    texto,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (diagnostico == null)
            {
                Console.WriteLine(
                    "No se pudo convertir la respuesta a DiagnosticoIA.");

                return null;
            }

            return diagnostico;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"ERROR IA IMAGEN: {ex}");

            return new DiagnosticoIA
            {
                Codigo = "IA_NO_DISPONIBLE",
                Servicio = "OpenAI",
                Problema = "El servicio de inteligencia artificial no está disponible actualmente."
            };
        }
    }
    

    private static async Task<byte[]> ReadStreamAsync(
        Stream stream)
    {
        using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }
}

#pragma warning restore OPENAI001