using Microsoft.Data.SqlClient;
using Gventas.WebMVC.Models;
using Gventas.WebMVC.Repositorio;

namespace Gventas.WebMVC.RepositorioSQL
{
    public class PedidoSQL : IPedidoRepositorio
    {
        private readonly string _cadena;
        public PedidoSQL(IConfiguration config) => _cadena = config.GetConnectionString("sql")!;

        public string RegistrarPedidoTransaccional(PedidoCabecera cabecera, List<PedidoDetalle> detalles)
        {
            using var cn = new SqlConnection(_cadena);
            cn.Open();
            SqlTransaction trx = cn.BeginTransaction();
            try
            {
                string sqlCab = "INSERT INTO tb_pedidos_cab (fecha, cod_cliente, total) VALUES (@fec, @cli, @tot); SELECT SCOPE_IDENTITY();";
                using var cmdCab = new SqlCommand(sqlCab, cn, trx);
                cmdCab.Parameters.AddWithValue("@fec", cabecera.fecha);
                cmdCab.Parameters.AddWithValue("@cli", cabecera.cod_cliente);
                cmdCab.Parameters.AddWithValue("@tot", detalles.Sum(d => d.subtotal));

                int idPedidoGenerado = Convert.ToInt32(cmdCab.ExecuteScalar());

                foreach (var item in detalles)
                {
                    string sqlDet = "INSERT INTO tb_pedidos_det (idpedido, cod_producto, cantidad, precio) VALUES (@id, @prod, @cant, @pre)";
                    using var cmdDet = new SqlCommand(sqlDet, cn, trx);
                    cmdDet.Parameters.AddWithValue("@id", idPedidoGenerado);
                    cmdDet.Parameters.AddWithValue("@prod", item.cod_producto);
                    cmdDet.Parameters.AddWithValue("@cant", item.cantidad);
                    cmdDet.Parameters.AddWithValue("@pre", item.precio);
                    cmdDet.ExecuteNonQuery();
                }

                trx.Commit();
                return $"Pedido N° {idPedidoGenerado} registrado exitosamente.";
            }
            catch (Exception ex)
            {
                trx.Rollback();
                return $"Error en la transacción: {ex.Message}";
            }
        }
    }
}
