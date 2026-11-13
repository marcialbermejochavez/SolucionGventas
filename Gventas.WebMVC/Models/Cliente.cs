using System.ComponentModel.DataAnnotations;

namespace Gventas.WebMVC.Models
{
    public class Cliente
    {
        [Required(ErrorMessage = "El código es obligatorio"), StringLength(5)]
        [Display(Name = "Código")]
        public string codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El RUC es obligatorio"), StringLength(11, MinimumLength = 11, ErrorMessage = "El RUC debe tener 11 dígitos")]
        [Display(Name = "RUC")]
        public string ruc { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombres / Razón Social")]
        public string nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio"), Display(Name = "Teléfono")]
        public string telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria"), Display(Name = "Dirección")]
        public string direccion { get; set; } = string.Empty;
    }
}
