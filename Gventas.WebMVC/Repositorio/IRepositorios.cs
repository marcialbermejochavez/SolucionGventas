using Gventas.WebMVC.Models;

namespace Gventas.WebMVC.Repositorio
{
    public interface IProductoRepositorio
    {
        IEnumerable<Producto> Listar();
        Producto? Buscar(string codigo);
        string GuardarMerge(Producto reg);
        string Eliminar(string codigo);
    }

    public interface IPedidoRepositorio
    {
        string RegistrarPedidoTransaccional(PedidoCabecera cabecera, List<PedidoDetalle> detalles);
    }
}
