using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarijaReadApp.Models;

public class Prestamo
{
    public int Id {get; set;}

    [Required]
    [Column(TypeName = "timestamp without time zone")]
    public DateTime FechaSalida { get; set; } = DateTime.Now;

    [Required]
    [Column(TypeName = "timestamp without time zone")]
    public DateTime FechaLimite { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? FechaDevolucion { get; set; }
    
    public int EjemplarId {get; set;}

    [ValidateNever]
    public virtual Ejemplar Ejemplar {get; set;} = null!;

    public int SocioId { get; set; }

    [ValidateNever]
    public virtual Socio Socio { get; set; } = null!;

    public int UsuarioId { get; set; }

    [ValidateNever]
    public virtual Usuario Usuario { get; set; } = null!;

    [ValidateNever]
    public virtual Multa? Multa { get; set; }

    [NotMapped]
    public string Descripcion => $"#{Id} - {Socio?.Nombre} - {Ejemplar?.Codigo}";
}