using Gventas.GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<ProductoGrpcService>();
app.MapGet("/", () => "Servicio gRPC de Productos Gventas activo. Use un cliente gRPC para comunicarse.");

app.Run();
