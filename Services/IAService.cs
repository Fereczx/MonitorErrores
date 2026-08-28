using OpenAI;
using OpenAI.Chat;

namespace MonitorErrores.Services;

public class IAService
{
    private readonly ChatClient _chatClient;

    public IAService(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"];

        _chatClient = new ChatClient(
            model: "gpt-4o-mini",
            apiKey: apiKey);
    }

    public async Task<string> AnalizarError(
        string codigo,
        string servicio,
        string mensaje,
        int erroresRecientes,
        bool esRecurrente)
    {
        var prompt = $"""
        Analiza el siguiente error de una aplicación de pagos.

        Código: {codigo}
        Servicio: {servicio}
        Mensaje: {mensaje}
        Errores similares en las últimas 2 horas: {erroresRecientes}
        Es recurrente: {esRecurrente}

        Explica de forma breve:
        1. Qué podría significar el error.
        2. Qué debería revisar el usuario o el equipo técnico.
        3. Si no hay información suficiente, indícalo claramente.

        No inventes una solución confirmada.
        """;

        ChatCompletion completion = await _chatClient.CompleteChatAsync(prompt);

        return completion.Content[0].Text;
    }
}