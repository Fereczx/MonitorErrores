using MonitorErrores.Data;
using MonitorErrores.Models;

namespace MonitorErrores.Services;

public class ErrorService
{
    private readonly AppDbContext _context;
    private readonly ErrorKnowledgeService _knowledgeService;

    public ErrorService(
        AppDbContext context,
        ErrorKnowledgeService knowledgeService)
    {
        _context = context;
        _knowledgeService = knowledgeService;
    }

    public Diagnostico ProcesarError(Error error)
    {
        // Primero buscamos los errores anteriores
        var haceDosHoras = DateTime.Now.AddHours(-2);

        var erroresRecientes = _context.Errores
            .Where(e =>
                e.Codigo == error.Codigo &&
                e.Servicio == error.Servicio &&
                e.Fecha >= haceDosHoras)
            .ToList();

        // Guardamos el error recibido
        _context.Errores.Add(error);
        _context.SaveChanges();

        // Creamos el diagnóstico
        var diagnostico = new Diagnostico
        {
            ErrorId = error.Id,
            Codigo = error.Codigo,
            Servicio = error.Servicio,
            ErroresRecientes = erroresRecientes.Count,
            EsRecurrente = erroresRecientes.Count >= 2,
            FechaDiagnostico = DateTime.Now
        };

        // Consultamos nuestra base de conocimientos
        var esConocido = _knowledgeService.ObtenerInformacion(
            error.Codigo,
            out var categoria,
            out var descripcion,
            out var recomendacion);

        if (esConocido)
        {
            diagnostico.EsConocido = true;
            diagnostico.Categoria = categoria;
            diagnostico.Descripcion = descripcion;
            diagnostico.Recomendacion = recomendacion;
        }
        else
        {
            diagnostico.EsConocido = false;
            diagnostico.Categoria = "Desconocido";
            diagnostico.Descripcion =
                "No se reconoce el código de error recibido.";

            diagnostico.Recomendacion =
                "No se puede determinar una solución para este error. " +
                "Por favor, comuníquese con Atención al Cliente de la aplicación.";
        }

        // Si ocurrió varias veces, lo indicamos en la recomendación
        if (diagnostico.EsRecurrente)
        {
            diagnostico.Recomendacion =
                $"Se detectaron {diagnostico.ErroresRecientes} errores similares " +
                $"en las últimas 2 horas. " +
                diagnostico.Recomendacion;
        }

        _context.Diagnosticos.Add(diagnostico);
        _context.SaveChanges();    
        
        return diagnostico;
    }

    public List<Error> ObtenerErrores()
    {
        return _context.Errores.ToList();
    }

    public List<Error> ObtenerErroresPorCodigo(string codigo)
    {
        return _context.Errores
            .Where(e => e.Codigo == codigo)
            .ToList();
    }

    public Diagnostico DiagnosticarError(Error error)
    {
        var haceDosHoras = DateTime.Now.AddHours(-2);

        var erroresRecientes = _context.Errores
            .Where(e =>
                e.Codigo == error.Codigo &&
                e.Servicio == error.Servicio &&
                e.Fecha >= haceDosHoras)
            .ToList();

        var diagnostico = new Diagnostico
        {
            Codigo = error.Codigo,
            Servicio = error.Servicio,
            ErroresRecientes = erroresRecientes.Count,
            EsRecurrente = erroresRecientes.Count >= 2
        };

        var esConocido = _knowledgeService.ObtenerInformacion(
            error.Codigo,
            out var categoria,
            out var descripcion,
            out var recomendacion);

        if (esConocido)
        {
            diagnostico.EsConocido = true;
            diagnostico.Categoria = categoria;
            diagnostico.Descripcion = descripcion;
            diagnostico.Recomendacion = recomendacion;
        }
        else
        {
            diagnostico.EsConocido = false;
            diagnostico.Categoria = "Desconocido";
            diagnostico.Descripcion =
                "No se reconoce el código de error recibido.";

            diagnostico.Recomendacion =
                "No se puede determinar una solución para este error. " +
                "Por favor, comuníquese con Atención al Cliente de la aplicación.";
        }

        if (diagnostico.EsRecurrente)
        {
            diagnostico.Recomendacion =
                $"Se detectaron {diagnostico.ErroresRecientes} errores similares " +
                $"en las últimas 2 horas. " +
                diagnostico.Recomendacion;
        }

        
        return diagnostico;
    }
}