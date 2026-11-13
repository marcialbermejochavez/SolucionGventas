using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Gventas.GrpcService;
using Gventas.WebMVC.Models;
using Gventas.WebMVC.Repositorio;

namespace Gventas.WebMVC.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IProductoRepositorio _repoProducto;
        private readonly string _grpcUrl;

        public ProductosController(IProductoRepositorio repoProducto, IConfiguration config)
        {
            _repoProducto = repoProducto;
            _grpcUrl = config["Servicios:UrlGrpc"]!;
        }

        public IActionResult Index(int p = 0)
        {
            var todos = _repoProducto.Listar().ToList();
            int filas = 5;
            int total = todos.Count;
            ViewBag.p = p;
            ViewBag.pags = total % filas == 0 ? total / filas : (total / filas) + 1;

            return View(todos.Skip(p * filas).Take(filas));
        }

        // Acción adicional que consume el servicio gRPC
        public async Task<IActionResult> CatalogoGrpc()
        {
            var lista = new List<Producto>();
            try
            {
                using var channel = GrpcChannel.ForAddress(_grpcUrl);
                var client = new ProductoGrpc.ProductoGrpcClient(channel);
                var reply = await client.ListarProductosAsync(new EmptyRequest());
                foreach (var item in reply.Items)
                {
                    lista.Add(new Producto
                    {
                        codigo = item.Codigo,
                        nombre = item.Nombre,
                        unidad = item.Unidad,
                        precio = Convert.ToDecimal(item.Precio)
                    });
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error conectando al servicio gRPC: " + ex.Message;
            }
            return View(lista);
        }

        [HttpGet]
        public IActionResult Edit(string? id)
        {
            var prod = string.IsNullOrEmpty(id) ? new Producto() : _repoProducto.Buscar(id);
            return View(prod);
        }

        [HttpPost]
        public IActionResult Edit(Producto reg)
        {
            if (!ModelState.IsValid) return View(reg);
            TempData["Mensaje"] = _repoProducto.GuardarMerge(reg);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(string id)
        {
            TempData["Mensaje"] = _repoProducto.Eliminar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
