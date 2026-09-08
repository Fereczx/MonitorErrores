# MonitorErrores

API desarrollada en C# y ASP.NET Core para el diagnóstico de errores
en aplicaciones de pagos.

El sistema recibe un error, busca primero una solución en una base
de conocimiento y, si no existe una solución confirmada, puede
utilizar Inteligencia Artificial para analizar el problema y buscar
información confiable.

Si no es posible determinar una solución completa y confiable,
el sistema deriva el caso a Atención al Cliente.


-------------------------------------------------------------------------------

## Objetivo

El objetivo de MonitorErrores es facilitar el diagnóstico de errores
provenientes de aplicaciones y servicios de pagos.

El sistema busca evitar respuestas incorrectas o especulativas
mediante un flujo de diagnóstico que prioriza las soluciones
previamente verificadas y la documentación confiable.


-------------------------------------------------------------------------------

## Flujo del sistema


ERROR
  |
  v
Base de conocimiento
  |
  v
¿Existe una solución confirmada?
  |
  +-- Sí --> Devolver solución
  |
  +-- No --> Consultar IA
                |
                v
          Buscar información
                |
                v
       ¿Solución completa?
          |
          +-- Sí --> Devolver solución
          |
          +-- No --> Atención al Cliente


-------------------------------------------------------------------------------

Características
Recepción de errores mediante API REST.
Búsqueda de soluciones en una base de conocimiento local.
Búsqueda por código y servicio.
Búsqueda por coincidencia del mensaje.
Búsqueda mediante palabras clave.
Normalización de texto.
Ignora diferencias entre mayúsculas y minúsculas.
Ignora diferencias de acentuación.
Validación de soluciones confirmadas.
Integración con OpenAI.
Preparación para búsqueda web mediante IA.
Priorización de documentación oficial.
Análisis de capturas de pantalla de errores.
Derivación a Atención al Cliente cuando no existe una solución
completa y confiable.
Pruebas unitarias mediante xUnit.


-------------------------------------------------------------------------------

Tecnologías
C#
.NET 10
ASP.NET Core Web API
OpenAI API
xUnit
PowerShell
Git / GitHub

El proyecto actualmente no utiliza SQL, SQLite ni Entity Framework
Core.


-------------------------------------------------------------------------------

Requisitos

Antes de ejecutar el proyecto se necesita:

.NET 10 SDK
Git
Visual Studio Code, Visual Studio u otro editor compatible
Una API Key de OpenAI para realizar consultas reales a la IA

La API de OpenAI es necesaria únicamente para las funcionalidades
que utilizan Inteligencia Artificial.


-------------------------------------------------------------------------------

Instalación

Clonar el repositorio:

git clone https://github.com/USUARIO/MonitorErrores.git

Ingresar al proyecto:

cd MonitorErrores

Restaurar los paquetes:

dotnet restore

Compilar el proyecto:

dotnet build


-------------------------------------------------------------------------------

Configuración de OpenAI

La API Key no debe almacenarse directamente dentro de
appsettings.json ni subirse al repositorio.

Durante el desarrollo se utilizan User Secrets.

Inicializar User Secrets:

dotnet user-secrets init

Configurar la API Key:

dotnet user-secrets set "OpenAI:ApiKey" "TU_API_KEY"

La integración con OpenAI requiere una cuenta con acceso a la API
y créditos/cuota disponibles.

-------------------------------------------------------------------------------

API
Diagnosticar un error
POST /api/errores

Ejemplo:

{
  "codigo": "TIMEOUT",
  "servicio": "Banco",
  "mensaje": "El banco no responde"
}

El sistema primero consulta la base de conocimiento.

Si encuentra una solución confirmada, devuelve dicha solución.

Si no encuentra una solución, consulta el servicio de Inteligencia
Artificial.

Si la IA no obtiene información suficiente para determinar una
solución completa y confiable, el usuario es derivado a Atención
al Cliente.

-------------------------------------------------------------------------------

Analizar una captura de pantalla
POST /api/errores/imagen

Este endpoint recibe una imagen mediante multipart/form-data.

La imagen puede contener una captura de pantalla de un error.

La Inteligencia Artificial intenta identificar:

Código del error.
Servicio involucrado.
Problema detectado.

Una vez identificado el error, continúa por el flujo normal de
diagnóstico.

-------------------------------------------------------------------------------

Base de conocimiento

La base de conocimiento se encuentra en:

Knowledge/soluciones.json

Contiene las soluciones conocidas por el sistema.

Una solución solamente puede ser utilizada como respuesta
definitiva cuando cumple las condiciones necesarias:

Está confirmada.
Tiene una fuente.
Tiene un tipo de fuente.
Tiene una solución concreta.

Las soluciones que todavía no fueron verificadas no se utilizan
como respuestas definitivas.

Búsqueda

El sistema utiliza diferentes métodos para encontrar soluciones.

Código + servicio

Primero intenta encontrar una solución mediante el código del error
y el servicio involucrado.

Coincidencia del mensaje

Si no encuentra una coincidencia exacta, analiza las palabras del
mensaje para encontrar problemas similares.

Palabras clave

También utiliza las palabras clave asociadas a cada solución.

La búsqueda normaliza el texto para ignorar diferencias de
mayúsculas, minúsculas y acentos.

-------------------------------------------------------------------------------

Inteligencia Artificial

Cuando no existe una solución confirmada en la base de conocimiento,
el sistema puede consultar OpenAI.

La integración está preparada para utilizar búsqueda web durante
el análisis.

La información debe priorizarse en el siguiente orden:

Documentación oficial.
Documentación del banco.
Documentación del proveedor de pagos.
Documentación técnica confiable.

La IA no debe:

Inventar información.
Inventar soluciones.
Presentar una solución parcial como definitiva.
Utilizar información contradictoria como una solución confirmada.

Una solución solamente se considera válida cuando existe
información suficiente para determinar qué ocurre y qué debe
hacerse para solucionarlo.

Si esto no es posible, el sistema deriva el caso a Atención al
Cliente.

-------------------------------------------------------------------------------

Testing

El proyecto cuenta con un proyecto independiente de pruebas:

MonitorErrores.Tests

Se utiliza xUnit para realizar pruebas unitarias.

Actualmente existen 13 pruebas.

Se prueban, entre otros casos:

Soluciones confirmadas.
Soluciones no confirmadas.
Soluciones sin fuente.
Soluciones sin texto.
Soluciones sin tipo de fuente.
Búsqueda por coincidencia del mensaje.
Búsqueda por palabras clave.
Separación por servicio.
Mayúsculas y minúsculas.
Acentos.
Prioridad de la base de conocimiento.
Consulta de IA cuando no existe una solución local.
Derivación a Atención al Cliente cuando la IA no obtiene una
solución completa.

Ejecutar las pruebas:

dotnet test ..\MonitorErrores.Tests\MonitorErrores.Tests.csproj

-------------------------------------------------------------------------------


Estructura del proyecto
MonitorErrores/
│
├── Controllers/
│   └── ErroresController.cs
│
├── Knowledge/
│   └── soluciones.json
│
├── Models/
│   ├── AnalisisErrorRequest.cs
│   ├── DiagnosticoIA.cs
│   ├── Error.cs
│   ├── ResultadoIA.cs
│   └── Solucion.cs
│
├── Properties/
│   └── launchSettings.json
│
├── Services/
│   ├── DiagnosticoService.cs
│   ├── ErrorKnowledgeService.cs
│   ├── ErrorService.cs
│   ├── IAService.cs
│   └── IIAService.cs
│
├── appsettings.json
├── appsettings.Development.json
├── MonitorErrores.csproj
├── MonitorErrores.http
├── Program.cs
└── README.md

-------------------------------------------------------------------------------

Aplicación externa
       |
       | POST /api/errores
       v
ErroresController
       |
       v
DiagnosticoService
       |
       v
ErrorService
       |
       v
ErrorKnowledgeService
       |
       +----------------------+
       |                      |
       v                      v
Solución encontrada      No encontrada
       |                      |
       v                      v
Devolver solución         IAService
                              |
                              v
                         OpenAI API
                              |
                              v
                       Búsqueda web
                              |
                              v
                    ¿Solución completa?
                         |
                  +------+------+
                  |             |
                 Sí            No
                  |             |
                  v             v
             Solución       Atención
                           al Cliente

-------------------------------------------------------------------------------

Seguridad

Las claves de API y otra información sensible no deben almacenarse
en el código fuente.

Durante el desarrollo se utilizan User Secrets para almacenar la
API Key de OpenAI.

No se deben subir credenciales, API Keys ni archivos que contengan
información sensible al repositorio.


-------------------------------------------------------------------------------

Estado del proyecto

🚧 En desarrollo

Backend
 API REST
 Recepción de errores
 Base de conocimiento
 Búsqueda por código y servicio
 Búsqueda por mensaje
 Búsqueda por palabras clave
 Normalización de texto
 Validación de soluciones confirmadas
 Servicio de diagnóstico
 Integración con OpenAI
 Preparación para búsqueda web
 Análisis de imágenes
 Tests unitarios
Frontend
 Interfaz de usuario
 Ingreso de errores
 Carga de capturas de pantalla
 Visualización del diagnóstico
 Visualización de la solución
 Mensaje de Atención al Cliente

-------------------------------------------------------------------------------

 Próximos pasos
Finalizar la limpieza y validación del backend.
Desarrollar la interfaz frontend.
Integrar el frontend con la API.
Realizar pruebas de integración.
Configurar créditos de OpenAI para realizar pruebas reales.
Verificar el comportamiento con errores reales de servicios de
pagos.
Incorporar y verificar soluciones reales en la base de
conocimiento.
                           
-------------------------------------------------------------------------------

Nota

Las soluciones actualmente incluidas en Knowledge/soluciones.json
son parte de la estructura inicial de la base de conocimiento y
deben ser verificadas antes de marcarse como soluciones confirmadas.

-------------------------------------------------------------------------------

