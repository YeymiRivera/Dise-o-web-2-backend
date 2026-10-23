using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TarijaReadApp.Models;

public class Socio
{
    public int Id {get; set;}

    [Required, StringLength(100)]
    public string Nombre {get; set;} = string.Empty;

    [Required, StringLength(15)]
    public string CI {get; set;} = string.Empty;

    [StringLength(100)]
    public string? Email {get; set;}

    [StringLength(20)]
    public string? Telefono {get; set;}

    [ValidateNever]
    public virtual ICollection<Prestamo> Prestamos {get; set;} = new HashSet<Prestamo>();
}