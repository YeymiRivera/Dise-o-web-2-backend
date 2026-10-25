using System.ComponentModel.DataAnnotations;

namespace TarijaReadApp.ViewModels;

public class CategoriaInput
{
    [Required, StringLength(50)]
    public string Nombre { get; set; } = string.Empty;
}