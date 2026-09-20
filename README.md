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

## Características

- Recepción de errores mediante API REST.
- Búsqueda de soluciones en una base de conocimiento local.
- Búsqueda por código y servicio.
- Búsqueda por coincidencia del mensaje.
- Búsqueda mediante palabras clave.
- Normalización de texto.
- Ignora diferencias entre mayúsculas y minúsculas.
- Ignora diferencias de acentuación.
- Validación de soluciones confirmadas.
- Integración con OpenAI.
- Búsqueda web mediante IA.
- Priorización de documentación oficial.
- Análisis de capturas de pantalla de errores.
- Interfaz web tipo chat.
- Envío de errores desde el frontend.
- Carga y vista previa de capturas.
- Ampliación de imágenes dentro de la interfaz.
- Visualización del diagnóstico y la solución.
- Indicador visual durante el análisis.
- Derivación a Atención al Cliente cuando no existe una solución completa y confiable.
- Pruebas unitarias mediante xUnit.


-------------------------------------------------------------------------------

## Tecnologías

C#
.NET 10
ASP.NET Core Web API
OpenAI API
HTML
CSS
JavaScript
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
## Ejecutar el frontend

El frontend se encuentra dentro de la carpeta:

MonitorErrores.Frontend/

Está desarrollado con HTML, CSS y JavaScript.

### 1. Ejecutar el backend

Primero hay que iniciar la API.

Desde la carpeta del proyecto:

dotnet run

2. Abrir el frontend

Con el backend ejecutándose, abrir el archivo:

MonitorErrores.Frontend/index.html

Se puede abrir directamente desde el explorador de archivos o utilizando una extensión como Live Server en Visual Studio Code.

3. Verificar la conexión

El frontend está configurado para comunicarse con:

http://localhost:5163/api/Errores

y para analizar imágenes utiliza:

http://localhost:5163/api/Errores/imagen

Por lo tanto, el backend debe estar ejecutándose para que el chat pueda enviar los errores y recibir el diagnóstico.

1. Ejecutar el backend
        ↓
   dotnet run
        ↓
2. Abrir index.html
        ↓
3. Utilizar el chat
        ↓
4. El frontend envía el error
        ↓
5. La API procesa el diagnóstico
        ↓
6. El frontend muestra el resultado

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

La integración utiliza búsqueda web durante el análisis cuando
es necesario obtener información externa.

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

Actualmente existen 14 pruebas.

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
Derivación a Atención al Cliente cuando la IA no está disponible.

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
├── MonitorErrores.Frontend/
│   ├── css/
│   │   └── style.css
│   ├── js/
│   │   └── app.js
│   └── index.html
├── appsettings.json
├── appsettings.Development.json
├── MonitorErrores.csproj
├── MonitorErrores.http
├── Program.cs
└── README.md

-------------------------------------------------------------------------------

## Arquitectura de la aplicación


Usuario
   |
   v
Frontend
(HTML / CSS / JavaScript)
   |
   | HTTP
   | POST /api/errores
   | POST /api/errores/imagen
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
Solución encontrada    No encontrada
   |                      |
   v                      v
Devolver solución      IAService
                           |
                           v
                      OpenAI API
                           |
                           v
                       Web Search
                           |
                           v
                    ¿Solución completa?
                       |
                  +----+----+
                  |         |
                 Sí        No
                  |         |
                  v         v
              Solución   Atención
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

## Estado del proyecto

🚧 En desarrollo

### Backend

- [x] API REST
- [x] Recepción de errores
- [x] Base de conocimiento
- [x] Búsqueda por código y servicio
- [x] Búsqueda por mensaje
- [x] Búsqueda por palabras clave
- [x] Normalización de texto
- [x] Validación de soluciones confirmadas
- [x] Servicio de diagnóstico
- [x] Integración con OpenAI
- [x] Búsqueda web mediante IA
- [x] Análisis de imágenes
- [x] Derivación a Atención al Cliente
- [x] Inyección de dependencias mediante interfaces
- [x] CORS
- [x] Tests unitarios

### Frontend

- [x] Interfaz de usuario
- [x] Interfaz de chat
- [x] Ingreso de errores
- [x] Envío de errores al backend
- [x] Carga de capturas de pantalla
- [x] Vista previa de imágenes
- [x] Ampliación de imágenes
- [x] Indicador de análisis
- [x] Visualización del diagnóstico
- [x] Visualización de la solución
- [x] Mensaje de Atención al Cliente

-------------------------------------------------------------------------------

## Próximos pasos

- Verificar soluciones reales en la base de conocimiento.
- Realizar pruebas completas de la integración con OpenAI.
- Realizar pruebas reales de análisis de imágenes con créditos disponibles.
- Mejorar la detección automática de códigos y servicios desde el frontend.
- Resolver la advertencia de seguridad relacionada con `Microsoft.OpenApi`.
- Realizar pruebas de integración completas.
- Ampliar los tests según nuevas funcionalidades.
- Verificar el comportamiento con errores reales de servicios de pagos.
                           
-------------------------------------------------------------------------------

Nota

Las soluciones actualmente incluidas en Knowledge/soluciones.json
son parte de la estructura inicial de la base de conocimiento y
deben ser verificadas antes de marcarse como soluciones confirmadas.

-------------------------------------------------------------------------------

