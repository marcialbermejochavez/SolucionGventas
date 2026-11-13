using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Gventas.WebAPI.Models;

namespace Gventas.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesAPIController : ControllerBase
    {
        private readonly string _cadena;
        public ClientesAPIController(IConfiguration config) => _cadena = config.GetConnectionString("sql")!;

        [HttpGet]
        public IActionResult GetClientes()
        {
            var lista = new List<Cliente>();
            using var cn = new SqlConnection(_cadena);
            cn.Open();
            using var cmd = new SqlCommand("usp_clientes_listar", cn) { CommandType = CommandType.StoredProcedure };
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Cliente
                {
                    codigo = dr.GetString(0),
                    ruc = dr.GetString(1),
                    nombres = dr.GetString(2),
                    telefono = dr.GetString(3),
                    direccion = dr.GetString(4)
                });
            }
            return Ok(lista);
        }

        [HttpGet("{codigo}")]
        public IActionResult GetCliente(string codigo)
        {
            Cliente? item = null;
            using var cn = new SqlConnection(_cadena);
            cn.Open();
            using var cmd = new SqlCommand("usp_clientes_buscar", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@codigo", codigo);
            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                item = new Cliente
                {
                    codigo = dr.GetString(0),
                    ruc = dr.GetString(1),
                    nombres = dr.GetString(2),
                    telefono = dr.GetString(3),
                    direccion = dr.GetString(4)
                };
            }
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public IActionResult PostCliente([FromBody] Cliente reg)
        {
            using var cn = new SqlConnection(_cadena);
            cn.Open();
            using var cmd = new SqlCommand("usp_clientes_merge", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@codigo", reg.codigo);
            cmd.Parameters.AddWithValue("@ruc", reg.ruc);
            cmd.Parameters.AddWithValue("@nombres", reg.nombres);
            cmd.Parameters.AddWithValue("@telefono", reg.telefono);
            cmd.Parameters.AddWithValue("@direccion", reg.direccion);
            cmd.ExecuteNonQuery();
            return Ok($"Cliente {reg.nombres} guardado vía Web API.");
        }

        [HttpDelete("{codigo}")]
        public IActionResult DeleteCliente(string codigo)
        {
            using var cn = new SqlConnection(_cadena);
            cn.Open();
            using var cmd = new SqlCommand("usp_clientes_eliminar", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@codigo", codigo);
            cmd.ExecuteNonQuery();
            return Ok($"Cliente {codigo} eliminado vía Web API.");
        }
    }
}
