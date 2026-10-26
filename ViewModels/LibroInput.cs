using System.ComponentModel.DataAnnotations;

namespace TarijaReadApp.ViewModels;

public class LibroInput
{
    [Required, StringLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Autor { get; set; } = string.Empty;

    [StringLength(20)]
    public string? ISBN { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una categoría.")]
    public int CategoriaId { get; set; }
}