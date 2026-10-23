using System.ComponentModel.DataAnnotations;

namespace TarijaReadApp.ViewModels;

public class MultaCreateViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un préstamo.")]
    [Display(Name = "Préstamo")]
    public int PrestamoId { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Range(0.01, 9999.99, ErrorMessage = "El monto debe ser un valor positivo entre 0.01 y 9999.99.")]
    public decimal Monto { get; set; }

    [Display(Name = "¿Pago realizado?")]
    public bool PagoRealizado { get; set; } = false;
}