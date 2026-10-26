using System.ComponentModel.DataAnnotations;

namespace TarijaReadApp.ViewModels;

public class SocioInput
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(15)]
    public string CI { get; set; } = string.Empty;

    [EmailAddress, StringLength(100)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Telefono { get; set; }
}