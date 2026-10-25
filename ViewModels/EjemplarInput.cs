using System.ComponentModel.DataAnnotations;
using TarijaReadApp.Models;

namespace TarijaReadApp.ViewModels;

public class EjemplarInput
{
    [Required, StringLength(30)]
    public string Codigo { get; set; } = string.Empty;

    public EstadoEjemplar Estado { get; set; } = EstadoEjemplar.Disponible;

    [Range(1, int.MaxValue)]
    public int LibroId { get; set; }
}