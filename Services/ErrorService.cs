using MonitorErrores.Models;

namespace MonitorErrores.Services;

public class ErrorService
{
    private readonly ErrorKnowledgeService _knowledgeService;

    public ErrorService(ErrorKnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;
    }

    public Solucion? BuscarSolucion(Error error)
    {
        return _knowledgeService.BuscarSolucion(
            error.Codigo,
            error.Servicio,
            error.Mensaje);
}
}