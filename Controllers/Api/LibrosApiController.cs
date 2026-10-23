using Microsoft.AspNetCore.Mvc;
using TarijaReadApp.Interfaces;
using TarijaReadApp.ViewModels;

namespace TarijaReadApp.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class LibrosApiController : ControllerBase
{
    private readonly ILibroRepository _repository;

    public LibrosApiController(ILibroRepository repository)
    {
        _repository = repository;
    }

    // GET: api/LibrosApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LibroDto>>> GetLibros()
    {
        var libros = await _repository.GetAllAsync(); 
        var dtos = libros.Select(l => new LibroDto(
            l.Id,
            l.Titulo,
            l.Autor,
            l.ISBN,
            l.Categoria?.Nombre ?? "Sin categoría"
        ));
        return Ok(dtos);
    }

    // GET: api/LibrosApi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<LibroDto>> GetLibro(int id)
    {
        var libro = await _repository.GetByIdAsync(id);
        if (libro == null) return NotFound();

        var dto = new LibroDto(libro.Id, libro.Titulo, libro.Autor, libro.ISBN, libro.Categoria?.Nombre ?? "Sin categoría");
        return Ok(dto);
    }
}