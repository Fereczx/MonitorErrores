namespace MonitorErrores.Models;

public class Solucion
{
    public string Codigo { get; set; } = "";

    public string Servicio { get; set; } = "";

    public string Problema { get; set; } = "";

    public string SolucionTexto { get; set; } = "";

    public string MensajeUsuario { get; set; } = "";

    public string Fuente { get; set; } = "";

    public string TipoFuente { get; set; } = "";

    public bool Confirmada { get; set; }

    public List<string> PalabrasClave { get; set; } = new();

    public DateTime? FechaVerificacion { get; set; }

    public string VerificadoPor { get; set; } = "";

    // public string FuenteUrl { get; set; } = "";
}