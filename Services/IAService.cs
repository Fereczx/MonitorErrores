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
        var prompt = $$"""
        Eres un asistente especializado en diagnosticar errores
        de aplicaciones y sistemas de pagos.

        Analiza el siguiente error:

        {{codigo}}
        {{servicio}}
        {{mensaje}}

        Tu objetivo es ayudar al usuario a resolver el problema
        siempre que exista información suficiente para hacerlo
        de forma segura y fundamentada.

        REGLAS DE ANÁLISIS:

        - Primero identifica qué significa el error.
        - Utiliza la búsqueda web cuando sea necesaria para verificar
        el significado del error o encontrar procedimientos de solución.
        - Prioriza documentación oficial del servicio, API, banco
        o proveedor involucrado.
        - Utiliza fuentes técnicas confiables cuando no exista
        documentación oficial específica.
        - No inventes códigos, causas, procedimientos ni fuentes.
        - Diferencia los hechos confirmados de las posibles causas.
        - No afirmes que una hipótesis es la causa definitiva.
        - No recomiendes repetir pagos cuando su estado sea incierto.
        - No solicites contraseñas, claves privadas ni datos completos
        de tarjetas.

        CUÁNDO OFRECER UNA SOLUCIÓN:

        Puedes proporcionar una solución automática cuando dispongas de
        información suficiente para recomendar pasos concretos, seguros
        y fundamentados.

        No es obligatorio conocer la causa interna exacta si existe un
        procedimiento de resolución seguro y respaldado por información fiable.

        La solución debe incluir pasos prácticos que el usuario pueda seguir.
        No te limites a describir el problema.

        Distingue entre dos tipos de respuesta:

        1. SOLUCIÓN ESPECÍFICA:
        Si encontraste información fiable que respalda los pasos para resolver
        el problema concreto, proporciona esos pasos.
        - "solucion": pasos concretos respaldados por información fiable.
        - "mensajeUsuario": explicación clara de la solución.
        - "solucionCompleta": true solo si la solución está suficientemente
        fundamentada para el caso presentado.

        2. ORIENTACIÓN PRELIMINAR:
        Si no puedes determinar la causa exacta o no encuentras una solución
        específica, pero conoces el significado general del error, puedes
        proporcionar una orientación general y segura.
        - Explica qué significa el error sin inventar causas.
        - Indica los pasos de comprobación que sean seguros y pertinentes.
        - Explica qué información falta para determinar la causa.
        - Si está relacionado con un pago de estado incierto, no recomiendes
        repetirlo hasta confirmar su estado con el proveedor.
        - "solucion": orientación general y pasos de comprobación seguros.
        - "mensajeUsuario": resumen claro de la orientación y de sus límites.
        - "solucionCompleta": false.

        Si no tienes información suficiente ni siquiera para proporcionar
        una orientación segura:
        - "solucion" debe ser una cadena vacía.
        - "mensajeUsuario" debe ser una cadena vacía.
        - "solucionCompleta" debe ser false.

        No inventes procedimientos, causas, documentación ni fuentes.
        
        FUENTES:

        - Incluye las fuentes realmente consultadas.
        - No inventes URLs ni atribuyas información a fuentes que
        no respaldan la recomendación.
        - Indica el tipo de fuente cuando pueda determinarse.

        FORMATO DE RESPUESTA:

        Responde ÚNICAMENTE con JSON válido con estos campos:

 
        {
        "problema": "descripción del problema",
        "solucion": "pasos concretos para resolverlo",
        "mensajeUsuario": "respuesta clara para el usuario",
        "fuente": "fuente o URL que respalda la solución",
        "tipoFuente": "tipo de fuente utilizada",
        "solucionCompleta": true
        }


        Si no existe una solución confiable, utiliza los valores
        vacíos y false indicados anteriormente.

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
                            Analiza la imagen para determinar si contiene evidencia visible de un error técnico relacionado con una aplicación, un sistema informático, una API o una operación de pago.

                            PRIMERA PRIORIDAD: VALIDAR LA IMAGEN

                            Antes de extraer información, determina si la imagen es pertinente para el diagnóstico.

                            Una imagen es pertinente si muestra evidencia visible de un error, por ejemplo:
                            - Un código de error HTTP o de una aplicación.
                            - Un mensaje de error, rechazo o fallo.
                            - Una operación de pago con un error explícito.
                            - Una pantalla de un sistema que indique claramente un problema técnico.

                            No consideres suficiente que la imagen simplemente muestre una tarjeta, dinero, una persona, un paisaje, un producto o una aplicación sin un error visible.

                            Si la imagen no está relacionada con un error técnico, o no existe evidencia suficiente para identificar uno, responde con los tres campos vacíos.

                            No inventes códigos, servicios, mensajes, causas ni información que no aparezca en la imagen.

                            EXTRACCIÓN

                            Si hay evidencia suficiente de un error:
                            - codigo: código visible del error. Si no aparece, utiliza una cadena vacía.
                            - servicio: servicio identificable mediante la imagen. Si no puede determinarse, utiliza una cadena vacía.
                            - problema: descripción breve basada únicamente en la evidencia visible. No inventes la causa.

                            FORMATO

                            Responde ÚNICAMENTE con JSON válido y utiliza exactamente esta estructura:

                            {
                            "codigo": "",
                            "servicio": "",
                            "problema": ""
                            }

                            Si la imagen no contiene un error técnico pertinente o no hay evidencia suficiente para diagnosticarlo, devuelve exactamente esa estructura con los tres campos vacíos.

                            No agregues texto antes ni después del JSON.
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

            // Rechazar imágenes sin evidencia suficiente de un error.
            if (
                string.IsNullOrWhiteSpace(diagnostico.Codigo) &&
                string.IsNullOrWhiteSpace(diagnostico.Servicio) &&
                string.IsNullOrWhiteSpace(diagnostico.Problema)
            )
            {
                Console.WriteLine(
                    "La imagen no contiene información suficiente para diagnosticar un error.");

                return new DiagnosticoIA
                {
                    Codigo = "IMAGEN_NO_VALIDA",
                    Servicio = "",
                    Problema = "La imagen no contiene evidencia suficiente de un error técnico."
                };
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