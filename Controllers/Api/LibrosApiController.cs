using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TarijaReadApp.Data;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;
using TarijaReadApp.ViewModels;

namespace TarijaReadApp.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class LibrosApiController : ControllerBase
{
    private readonly ILibroRepository _repository;
    private readonly ApplicationDbContext _context;

    public LibrosApiController(ILibroRepository repository, ApplicationDbContext context)
    {
        _repository = repository;
        _context = context;
    }

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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetLibro(int id)
    {
        var libro = await _repository.GetByIdAsync(id);
        if (libro == null) return NotFound();

        return Ok(new
        {
            id = libro.Id,
            titulo = libro.Titulo,
            autor = libro.Autor,
            isbn = libro.ISBN,
            categoria = libro.Categoria?.Nombre ?? "Sin categoría",
            categoriaId = libro.CategoriaId
        });
    }

    
    [HttpPost("CreateLibro")]
    public async Task<IActionResult> CreateLibro([FromBody] LibroInput input)
    {
        bool categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == input.CategoriaId);
        if (!categoriaExiste)
            return BadRequest("La categoría seleccionada no existe.");

        var libro = new Libro
        {
            Titulo = input.Titulo.Trim(),
            Autor = input.Autor.Trim(),
            ISBN = string.IsNullOrWhiteSpace(input.ISBN) ? null : input.ISBN.Trim(),
            CategoriaId = input.CategoriaId
        };

        _context.Libros.Add(libro);
        await _context.SaveChangesAsync();

        return Ok(new { id = libro.Id, titulo = libro.Titulo, autor = libro.Autor, isbn = libro.ISBN, categoriaId = libro.CategoriaId });
    }

    
    [HttpPut("EditLibro/{id:int}")]
    public async Task<IActionResult> EditLibro(int id, [FromBody] LibroInput input)
    {
        var libro = await _context.Libros.FindAsync(id);
        if (libro == null) return NotFound();

        bool categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == input.CategoriaId);
        if (!categoriaExiste)
            return BadRequest("La categoría seleccionada no existe.");

        libro.Titulo = input.Titulo.Trim();
        libro.Autor = input.Autor.Trim();
        libro.ISBN = string.IsNullOrWhiteSpace(input.ISBN) ? null : input.ISBN.Trim();
        libro.CategoriaId = input.CategoriaId;

        await _context.SaveChangesAsync();

        return Ok(new { id = libro.Id, titulo = libro.Titulo, autor = libro.Autor, isbn = libro.ISBN, categoriaId = libro.CategoriaId });
    }

    
    [HttpDelete("EliminarLibro/{id:int}")]
    public async Task<IActionResult> EliminarLibro(int id)
    {
        var libro = await _context.Libros.FindAsync(id);
        if (libro == null) return NotFound();

        bool tieneEjemplares = await _context.Ejemplares.AnyAsync(e => e.LibroId == id);
        if (tieneEjemplares)
            return Conflict("No se puede eliminar: el libro tiene ejemplares asociados.");

        _context.Libros.Remove(libro);
        await _context.SaveChangesAsync();

        return Ok();
    }
}