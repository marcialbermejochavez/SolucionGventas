using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Gventas.WebMVC.Hubs;
using Gventas.WebMVC.Models;
using Gventas.WebMVC.Repositorio;

namespace Gventas.WebMVC.Controllers
{
    public class PedidosController : Controller
    {
        private readonly IProductoRepositorio _repoProd;
        private readonly IPedidoRepositorio _repoPedido;
        private readonly IHubContext<NotificacionHub> _hubContext;
        private readonly string _apiClientes;

        public PedidosController(IProductoRepositorio repoProd, IPedidoRepositorio repoPedido, IHubContext<NotificacionHub> hubContext, IConfiguration config)
        {
            _repoProd = repoProd;
            _repoPedido = repoPedido;
            _hubContext = hubContext;
            _apiClientes = config["Servicios:UrlWebApi"]!;
        }

        public async Task<IActionResult> Crear()
        {
            var vm = new PedidoViewModel
            {
                ListaProductos = _repoProd.Listar().ToList(),
                Detalles = ObtenerCarritoSesion()
            };

            try
            {
                using var http = new HttpClient();
                var resp = await http.GetAsync(_apiClientes);
                if (resp.IsSuccessStatusCode)
                    vm.ListaClientes = JsonConvert.DeserializeObject<List<Cliente>>(await resp.Content.ReadAsStringAsync())!;
            }
            catch { }

            return View(vm);
        }

        [HttpPost]
        public IActionResult AgregarDetalle(string cod_producto, int cantidad)
        {
            var prod = _repoProd.Buscar(cod_producto);
            if (prod != null && cantidad > 0)
            {
                var carrito = ObtenerCarritoSesion();
                var existente = carrito.FirstOrDefault(x => x.cod_producto == cod_producto);
                if (existente != null)
                    existente.cantidad += cantidad;
                else
                    carrito.Add(new PedidoDetalle
                    {
                        cod_producto = prod.codigo,
                        nombre_producto = prod.nombre,
                        cantidad = cantidad,
                        precio = prod.precio
                    });
                HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(carrito));
            }
            return RedirectToAction(nameof(Crear));
        }

        public IActionResult QuitarDetalle(string id)
        {
            var carrito = ObtenerCarritoSesion();
            carrito.RemoveAll(x => x.cod_producto == id);
            HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(carrito));
            return RedirectToAction(nameof(Crear));
        }

        [HttpPost]
        public async Task<IActionResult> GrabarPedido(string cod_cliente)
        {
            var carrito = ObtenerCarritoSesion();
            if (!carrito.Any())
            {
                TempData["Mensaje"] = "Debe agregar al menos un producto al pedido.";
                return RedirectToAction(nameof(Crear));
            }

            var cabecera = new PedidoCabecera { cod_cliente = cod_cliente, fecha = DateTime.Now };
            string resultado = _repoPedido.RegistrarPedidoTransaccional(cabecera, carrito);
            TempData["Mensaje"] = resultado;

            decimal totalPedido = carrito.Sum(x => x.subtotal);
            await _hubContext.Clients.All.SendAsync("RecibirNotificacion", $"Nuevo pedido ({resultado}) - Cliente: {cod_cliente} - Total: S/ {totalPedido:F2}");

            HttpContext.Session.Remove("Carrito");
            return RedirectToAction(nameof(Crear));
        }

        private List<PedidoDetalle> ObtenerCarritoSesion()
        {
            var json = HttpContext.Session.GetString("Carrito");
            return string.IsNullOrEmpty(json) ? new List<PedidoDetalle>() : JsonConvert.DeserializeObject<List<PedidoDetalle>>(json)!;
        }
    }
}
