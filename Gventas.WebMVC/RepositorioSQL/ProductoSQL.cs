using Microsoft.Data.SqlClient;
using System.Data;
using Gventas.WebMVC.Models;
using Gventas.WebMVC.Repositorio;

namespace Gventas.WebMVC.RepositorioSQL
{
    public class ProductoSQL : IProductoRepositorio
    {
        private readonly string _cadena;
        public ProductoSQL(IConfiguration config) => _cadena = config.GetConnectionString("sql")!;

        public IEnumerable<Producto> Listar()
        {
            var lista = new List<Producto>();
            using var cn = new SqlConnection(_cadena);
            cn.Open();
            using var cmd = new SqlCommand("usp_productos_listar", cn) { CommandType = CommandType.StoredProcedure };
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Producto
                {
                    codigo = dr.GetString(0),
                    nombre = dr.GetString(1),
                    unidad = dr.GetString(2),
                    precio = dr.GetDecimal(3)
                });
            }
            return lista;
        }

        public Producto? Buscar(string codigo) => Listar().FirstOrDefault(p => p.codigo == codigo);

        public string GuardarMerge(Producto reg)
        {
            using var cn = new SqlConnection(_cadena);
            cn.Open();
            using var cmd = new SqlCommand("usp_productos_merge", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@codigo", reg.codigo);
            cmd.Parameters.AddWithValue("@nombre", reg.nombre);
            cmd.Parameters.AddWithValue("@unidad", reg.unidad);
            cmd.Parameters.AddWithValue("@precio", reg.precio);
            cmd.ExecuteNonQuery();
            return $"Producto {reg.nombre} guardado correctamente.";
        }

        public string Eliminar(string codigo)
        {
            using var cn = new SqlConnection(_cadena);
            cn.Open();
            using var cmd = new SqlCommand("usp_productos_eliminar", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@codigo", codigo);
            cmd.ExecuteNonQuery();
            return $"Producto {codigo} eliminado.";
        }
    }
}
