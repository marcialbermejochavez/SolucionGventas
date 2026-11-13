using Grpc.Core;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Gventas.GrpcService.Services
{
    public class ProductoGrpcService : ProductoGrpc.ProductoGrpcBase
    {
        private readonly string _cadena;
        public ProductoGrpcService(IConfiguration config) => _cadena = config.GetConnectionString("sql")!;

        public override Task<ListaProductosResponse> ListarProductos(EmptyRequest request, ServerCallContext context)
        {
            var response = new ListaProductosResponse();
            using var cn = new SqlConnection(_cadena);
            cn.Open();
            using var cmd = new SqlCommand("usp_productos_listar", cn) { CommandType = CommandType.StoredProcedure };
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                response.Items.Add(new ProductoItem
                {
                    Codigo = dr.GetString(0),
                    Nombre = dr.GetString(1),
                    Unidad = dr.GetString(2),
                    Precio = Convert.ToDouble(dr.GetDecimal(3))
                });
            }
            return Task.FromResult(response);
        }
    }
}
