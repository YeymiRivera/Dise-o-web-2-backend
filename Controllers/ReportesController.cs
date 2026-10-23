using Microsoft.AspNetCore.Mvc;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Services;
using TarijaReadApp.ViewModels;

namespace TarijaReadApp.Controllers;

public class ReportesController : Controller
{
    private readonly ILibroRepository _libroRepository;
    private readonly ReportService _reportService;

    public ReportesController(ILibroRepository libroRepository, ReportService reportService)
    {
        _libroRepository = libroRepository;
        _reportService = reportService;
    }

    public IActionResult Dashboard() => View();

    [HttpGet]
    public async Task<IActionResult> DescargarExcel()
    {
        var libros = await ObtenerLibrosDto();
        var contenido = _reportService.GenerarExcelLibros(libros);
        return File(contenido,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "CatalogoLibros.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> DescargarPdf()
    {
        var libros = await ObtenerLibrosDto();
        var contenido = _reportService.GenerarPdfLibros(libros);
        return File(contenido, "application/pdf", "CatalogoLibros.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> DatosGrafico()
    {
        // LINQ: agrupar libros por categoría y contar (optimizado con AsNoTracking desde el repositorio)
        var libros = await _libroRepository.GetAllAsync();

        var datos = libros
            .GroupBy(l => l.Categoria != null ? l.Categoria.Nombre : "Sin categoría")
            .Select(g => new { etiqueta = g.Key, valor = g.Count() })
            .ToList();

        return Json(datos);
    }

    private async Task<List<LibroDto>> ObtenerLibrosDto()
    {
        var libros = await _libroRepository.GetAllAsync();
        return libros.Select(l => new LibroDto(
            l.Id, l.Titulo, l.Autor, l.ISBN, l.Categoria?.Nombre ?? "Sin categoría"
        )).ToList();
    }
}