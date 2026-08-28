using MonitorErrores.Models;

namespace MonitorErrores.Services;

public class ErrorKnowledgeService
{
    public bool ObtenerInformacion(
        string codigo,
        out string categoria,
        out string descripcion,
        out string recomendacion)
    {
        categoria = "";
        descripcion = "";
        recomendacion = "";

        if (codigo == "TIMEOUT")
        {
            categoria = "Comunicación";
            descripcion =
                "El servicio externo no respondió dentro del tiempo esperado.";
            recomendacion =
                "Verificar la disponibilidad del servicio y reintentar la operación.";

            return true;
        }

        if (codigo == "PAYMENT_DECLINED")
        {
            categoria = "Pago";
            descripcion =
                "La operación de pago fue rechazada.";
            recomendacion =
                "Verificar los datos de la operación o utilizar otro medio de pago.";

            return true;
        }

        if (codigo == "AUTH_ERROR")
        {
            categoria = "Autenticación";
            descripcion =
                "No fue posible autenticar la solicitud.";
            recomendacion =
                "Verificar las credenciales y los permisos de acceso.";

            return true;
        }

        if (codigo == "CONNECTION_ERROR")
        {
            categoria = "Conexión";
            descripcion =
                "No fue posible establecer una conexión con el servicio externo.";
            recomendacion =
                "Verificar la conexión y comprobar si el servicio externo está disponible.";

            return true;
        }

        return false;
    }
}