namespace Gventas.WebMVC.Models
{
    public class PedidoCabecera
    {
        public int idpedido { get; set; }
        public DateTime fecha { get; set; } = DateTime.Now;
        public string cod_cliente { get; set; } = string.Empty;
        public decimal total { get; set; }
    }

    public class PedidoDetalle
    {
        public string cod_producto { get; set; } = string.Empty;
        public string nombre_producto { get; set; } = string.Empty;
        public int cantidad { get; set; }
        public decimal precio { get; set; }
        public decimal subtotal => cantidad * precio;
    }

    public class PedidoViewModel
    {
        public PedidoCabecera Cabecera { get; set; } = new PedidoCabecera();
        public List<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
        public List<Cliente> ListaClientes { get; set; } = new List<Cliente>();
        public List<Producto> ListaProductos { get; set; } = new List<Producto>();
    }
}
