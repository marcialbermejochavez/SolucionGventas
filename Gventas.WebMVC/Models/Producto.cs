using System.ComponentModel.DataAnnotations;

namespace Gventas.WebMVC.Models
{
    public class Producto
    {
        [Required(ErrorMessage = "Código requerido"), StringLength(5), Display(Name = "Código")]
        public string codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nombre requerido"), Display(Name = "Nombre del Producto")]
        public string nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unidad requerida"), Display(Name = "Unidad de Medida")]
        public string unidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Precio requerido"), Range(0.1, 99999), Display(Name = "Precio Unitario")]
        public decimal precio { get; set; }
    }
}
