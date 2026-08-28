namespace MonitorErrores.Models;

public class Error
{
    public int Id { get; set; }

    public string Codigo { get; set; } = "";

    public string Servicio { get; set; } = "";

    public string Mensaje { get; set; } = "";

    public DateTime Fecha { get; set; }

    public string Estado { get; set; } = "";
    public string Categoria { get; set; } = "";
    public string Descripcion { get; set; } = "";

    public string Recomendacion { get; set; } = "";
}