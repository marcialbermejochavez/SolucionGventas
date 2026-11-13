using Gventas.WebMVC.Hubs;
using Gventas.WebMVC.Repositorio;
using Gventas.WebMVC.RepositorioSQL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// Inyección de Dependencias
builder.Services.AddScoped<IProductoRepositorio, ProductoSQL>();
builder.Services.AddScoped<IPedidoRepositorio, PedidoSQL>();

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Productos}/{action=Index}/{id?}");

app.MapHub<NotificacionHub>("/notificacionHub");

app.Run();
