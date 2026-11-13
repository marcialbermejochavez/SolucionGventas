# Solución Integradora: Sistema de Gestión Comercial (Gventas)
Proyecto en **ASP.NET Core (.NET 8.0)** que integra servicios distribuidos y arquitectura empresarial:
1. **Gventas.WebAPI**: Servicio RESTful con CORS y ADO.NET para el CRUD de Clientes.
2. **Gventas.GrpcService**: Servicio gRPC con Protocol Buffers (`productos.proto`) para consulta rápida de Productos.
3. **Gventas.WebMVC**: Aplicación principal MVC con Razor (`_LayoutMaster`), Inyección de Dependencias, Transacciones SQL (`SqlTransaction`) para Pedidos Cabecera-Detalle, cliente `HttpClient`, cliente `GrpcChannel`, y **SignalR** en tiempo real.

## Pasos para ejecutar en Visual Studio 2022
1. Abre **SQL Server Management Studio (SSMS)** y ejecuta el archivo `DB_Gventas.sql`.
2. Revisa la cadena de conexión `"sql"` en `Gventas.WebAPI/appsettings.json`, `Gventas.GrpcService/appsettings.json` y `Gventas.WebMVC/appsettings.json` (por defecto apunta a `server=.;`).
3. Abre `SolucionGventas.sln` en Visual Studio 2022.
4. Haz clic derecho sobre la **Solución** -> **Propiedades** -> **Proyectos de inicio múltiples**.
5. Selecciona **Iniciar** para los 3 proyectos (`Gventas.WebAPI`, `Gventas.GrpcService`, `Gventas.WebMVC`).
6. Presiona `F5` para compilar, restaurar paquetes NuGet y ejecutar la solución.
