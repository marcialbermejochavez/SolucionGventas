using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Gventas.WebMVC.Models;

namespace Gventas.WebMVC.Controllers
{
    public class ClientesController : Controller
    {
        private readonly string _apiUrl;
        public ClientesController(IConfiguration config) => _apiUrl = config["Servicios:UrlWebApi"]!;

        public async Task<IActionResult> Index()
        {
            var lista = new List<Cliente>();
            try
            {
                using var http = new HttpClient();
                var response = await http.GetAsync(_apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    lista = JsonConvert.DeserializeObject<List<Cliente>>(json)!;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudo conectar con Gventas.WebAPI: " + ex.Message;
            }
            return View(lista);
        }

        [HttpGet]
        public IActionResult Create() => View(new Cliente());

        [HttpPost]
        public async Task<IActionResult> Create(Cliente reg)
        {
            if (!ModelState.IsValid) return View(reg);

            using var http = new HttpClient();
            var contenido = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
            var response = await http.PostAsync(_apiUrl, contenido);
            TempData["Mensaje"] = await response.Content.ReadAsStringAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            using var http = new HttpClient();
            var response = await http.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            var reg = JsonConvert.DeserializeObject<Cliente>(await response.Content.ReadAsStringAsync());
            return View(reg);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Cliente reg)
        {
            if (!ModelState.IsValid) return View(reg);
            using var http = new HttpClient();
            var contenido = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
            var response = await http.PostAsync(_apiUrl, contenido);
            TempData["Mensaje"] = await response.Content.ReadAsStringAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            using var http = new HttpClient();
            var response = await http.DeleteAsync($"{_apiUrl}/{id}");
            TempData["Mensaje"] = await response.Content.ReadAsStringAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
