using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TarijaReadApp.Models;

public class Ejemplar
{
    public int Id {get; set;}

    [Required, StringLength(30)]
    public string Codigo {get; set;} = string.Empty;

    [Required]
    public EstadoEjemplar Estado {get; set;} = EstadoEjemplar.Disponible;

    public int LibroId {get; set;}

    [ValidateNever]
    public virtual Libro Libro {get; set;} = null!;

    [ValidateNever]
    public virtual ICollection<Prestamo> Prestamos {get; set;} = new HashSet<Prestamo>();
}