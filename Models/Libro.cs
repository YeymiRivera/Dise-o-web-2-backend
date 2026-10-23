using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TarijaReadApp.Models;

public class Libro
{
    public int Id {get; set;}

    [Required, StringLength(150)]
    public string Titulo {get; set;} = string.Empty;

    [Required, StringLength(100)]
    public string Autor {get; set;} = string.Empty;

    [StringLength(20)]
    public string? ISBN {get; set;}

    public int CategoriaId {get; set;}

    [ValidateNever]
    public virtual Categoria Categoria {get; set;} = null!;

    [ValidateNever]
    public virtual ICollection<Ejemplar> Ejemplares {get; set;} = new HashSet<Ejemplar>();
}