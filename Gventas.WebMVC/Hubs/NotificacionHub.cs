using Microsoft.AspNetCore.SignalR;

namespace Gventas.WebMVC.Hubs
{
    public class NotificacionHub : Hub
    {
        public async Task NotificarPedido(string cliente, decimal total)
        {
            await Clients.All.SendAsync("RecibirNotificacion", $"Nuevo pedido registrado para {cliente} por S/ {total:F2}");
        }
    }
}
