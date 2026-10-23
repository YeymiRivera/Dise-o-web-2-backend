using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TarijaReadApp.Models;

public class Categoria
{
    public int Id {get; set;}
    [Required, StringLength(50)]
    public string Nombre {get; set;} = string.Empty;

    [ValidateNever]
    public virtual ICollection<Libro> Libros {get; set;} = new HashSet<Libro>();
}