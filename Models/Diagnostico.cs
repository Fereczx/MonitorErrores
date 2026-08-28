namespace MonitorErrores.Models;

public class Diagnostico
{
    public int Id { get; set; }

    public int ErrorId { get; set; }

    public string Codigo { get; set; } = "";

    public string Servicio { get; set; } = "";

    public bool EsConocido { get; set; }

    public string Categoria { get; set; } = "";

    public string Descripcion { get; set; } = "";

    public int ErroresRecientes { get; set; }

    public bool EsRecurrente { get; set; }

    public string NivelGravedad { get; set; } = "";

    public string Recomendacion { get; set; } = "";

    public DateTime FechaDiagnostico { get; set; }
}