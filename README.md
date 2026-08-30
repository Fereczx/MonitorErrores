# MonitorErrores

API desarrollada en C# y ASP.NET Core para el monitoreo y diagnóstico de errores de aplicaciones.

El sistema permite registrar errores, identificar errores conocidos, detectar errores recurrentes y almacenar un historial de diagnósticos.


## Características

- Registro de errores.
- Persistencia de errores mediante SQLite.
- Identificación de errores conocidos.
- Clasificación de errores.
- Detección de errores recurrentes.
- Historial de diagnósticos.
- API REST.
- Integración preparada con OpenAI.
- Pruebas mediante PowerShell.


## Tecnologías

- C#
- .NET 10
- ASP.NET Core
- Entity Framework Core
- SQLite
- OpenAI API
- PowerShell

## Requisitos

Antes de ejecutar el proyecto se necesita:

- .NET 10 SDK
- Git
- Un editor como Visual Studio o Visual Studio Code

- Una API Key de OpenAI (opcional mientras la IA se encuentre en desarrollo)

## Instalación

Clonar el repositorio:

git clone https://github.com/USUARIO/MonitorErrores.git

Ingresar al proyecto:

cd MonitorErrores


Restaurar los paquetes:

dotnet restore

dotnet build

## Base de datos

El proyecto utiliza SQLite.

La base de datos se crea mediante Entity Framework Core.

Para aplicar las migraciones:

dotnet ef database update

Las tablas son creadas automáticamente a partir de las migraciones existentes.



## Configuración de OpenAI

La API Key no debe almacenarse dentro de `appsettings.json`
ni subirse al repositorio.

Para configurar la API Key utilizando User Secrets:

dotnet user-secrets init

dotnet user-secrets set "OpenAI:ApiKey" "TU_API_KEY"

La integración con OpenAI requiere una cuenta con acceso a la API y cuota disponible.



## Ejecutar

Desde la carpeta del proyecto:

dotnet run

aparecerá algo como:

Now listening on: http://localhost:5163


## API

### Registrar y procesar un error

POST /api/errores

ejemplo:

{
  "codigo": "TIMEOUT",
  "servicio": "Banco",
  "mensaje": "Servidor no respondio",
  "fecha": "2026-08-27T21:30:00",
  "estado": "Nuevo"
}

respuesta:

{
  "id": 1,
  "errorId": 9,
  "codigo": "TIMEOUT",
  "servicio": "Banco",
  "esConocido": true,
  "categoria": "Comunicación",
  "erroresRecientes": 0,
  "esRecurrente": false
}

### Obtener todos los errores

GET /api/errores

### Obtener errores por código

GET /api/errores/{codigo}

### Diagnosticar un error

POST /api/errores/diagnosticar


PowerShell listos para copiar:

$body = @{
    codigo = "TIMEOUT"
    servicio = "Banco"
    mensaje = "Servidor no respondio"
    fecha = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    estado = "Nuevo"
} | ConvertTo-Json

$body = @{
    codigo = "ERROR_8472"
    servicio = "Banco"
    mensaje = "Ocurrio un error inesperado"
    fecha = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    estado = "Nuevo"
} | ConvertTo-Json


Invoke-RestMethod `
    -Uri "http://localhost:5163/api/errores" `
    -Method Post `
    -ContentType "application/json; charset=utf-8" `
    -Body $body



MonitorErrores/
│
├── Controllers/
│   └── ErroresController.cs
│
├── Data/
│   └── AppDbContext.cs
│
├── Models/
│   ├── Error.cs
│   └── Diagnostico.cs
│
├── Services/
│   ├── ErrorService.cs
│   ├── ErrorKnowledgeService.cs
│   └── IAService.cs
│
├── Migrations/
│
├── Program.cs
├── appsettings.json
├── MonitorErrores.csproj
└── README.md



Aplicación externa
       │
       │ POST /api/errores
       ▼
ErroresController
       │
       ▼
ErrorService
       │
       ├──────────────┐
       ▼              ▼
ErrorKnowledge    SQLite
Service           │
       │          └── Errores
       │              Diagnosticos
       │
       ▼
   Diagnóstico
       │
       ▼
    IAService
       │
       ▼
   OpenAI API



## Estado del proyecto

🚧 En desarrollo

### Implementado

- [x] API REST
- [x] Registro de errores
- [x] SQLite
- [x] Entity Framework Core
- [x] Base de conocimiento
- [x] Detección de errores recurrentes
- [x] Historial de diagnósticos
- [x] Integración inicial con OpenAI

### Próximamente

- [ ] Niveles de gravedad
- [ ] Dashboard visual
- [ ] Integración completa de IA
- [ ] Estadísticas
- [ ] Filtros y búsqueda


   

