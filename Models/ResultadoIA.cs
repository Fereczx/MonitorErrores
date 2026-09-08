namespace MonitorErrores.Models;

public class ResultadoIA
{
    public string Problema { get; set; } = "";

    public string Solucion { get; set; } = "";

    public string MensajeUsuario { get; set; } = "";

    public string Fuente { get; set; } = "";

    public string TipoFuente { get; set; } = "";

    public bool SolucionCompleta { get; set; }
}