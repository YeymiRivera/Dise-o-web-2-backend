using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TarijaReadApp.Data;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;
using TarijaReadApp.ViewModels;

namespace TarijaReadApp.Controllers.Api;

[ApiController]
[Route("api/Ejemplares")]
public class EjemplaresApiController : ControllerBase
{
    private readonly IEjemplarRepository _repository;
    private readonly ApplicationDbContext _context;

    public EjemplaresApiController(IEjemplarRepository repository, ApplicationDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    // GET: api/Ejemplares/GetEjemplares
    [HttpGet("GetEjemplares")]
    public async Task<IActionResult> GetEjemplares()
    {
        var ejemplares = await _repository.GetAllAsync(); 

        var resultado = ejemplares
            .OrderBy(e => e.Codigo)
            .Select(e => new
            {
                id = e.Id,
                codigo = e.Codigo,
                estado = (int)e.Estado, 
                libroId = e.LibroId,
                libro = e.Libro == null ? null : new { id = e.Libro.Id, titulo = e.Libro.Titulo }
            });

        return Ok(resultado);
    }

    // GET: api/Ejemplares/GetEjemplar/5
    [HttpGet("GetEjemplar/{id:int}")]
    public async Task<IActionResult> GetEjemplar(int id)
    {
        var e = await _repository.GetByIdAsync(id);
        if (e == null) return NotFound();

        return Ok(new
        {
            id = e.Id,
            codigo = e.Codigo,
            estado = (int)e.Estado,
            libroId = e.LibroId,
            libro = e.Libro == null ? null : new { id = e.Libro.Id, titulo = e.Libro.Titulo }
        });
    }

    // POST: api/Ejemplares/CreateEjemplar
    [HttpPost("CreateEjemplar")]
    public async Task<IActionResult> CreateEjemplar([FromBody] EjemplarInput input)
    {
        var codigo = input.Codigo.Trim();

        if (!await _context.Libros.AnyAsync(l => l.Id == input.LibroId))
            return BadRequest("El libro seleccionado no existe.");

        if (await _context.Ejemplares.AnyAsync(e => e.Codigo.ToLower() == codigo.ToLower()))
            return Conflict("Ya existe un ejemplar con ese código.");

        var ejemplar = new Ejemplar
        {
            Codigo = codigo,
            Estado = input.Estado,
            LibroId = input.LibroId
        };

        await _repository.AddAsync(ejemplar);
        await _repository.SaveChangesAsync();

        return Ok(new { id = ejemplar.Id, codigo = ejemplar.Codigo, estado = (int)ejemplar.Estado, libroId = ejemplar.LibroId });
    }

    // PUT: api/Ejemplares/EditEjemplar/5
    [HttpPut("EditEjemplar/{id:int}")]
    public async Task<IActionResult> EditEjemplar(int id, [FromBody] EjemplarInput input)
    {
        var ejemplar = await _context.Ejemplares.FindAsync(id);
        if (ejemplar == null) return NotFound();

        var codigo = input.Codigo.Trim();

        if (!await _context.Libros.AnyAsync(l => l.Id == input.LibroId))
            return BadRequest("El libro seleccionado no existe.");

        if (await _context.Ejemplares.AnyAsync(e => e.Id != id && e.Codigo.ToLower() == codigo.ToLower()))
            return Conflict("Ya existe otro ejemplar con ese código.");

        ejemplar.Codigo = codigo;
        ejemplar.Estado = input.Estado;
        ejemplar.LibroId = input.LibroId;
        await _context.SaveChangesAsync();

        return Ok(new { id = ejemplar.Id, codigo = ejemplar.Codigo, estado = (int)ejemplar.Estado, libroId = ejemplar.LibroId });
    }

    // DELETE: api/Ejemplares/EliminarEjemplar/5
    [HttpDelete("EliminarEjemplar/{id:int}")]
    public async Task<IActionResult> EliminarEjemplar(int id)
    {
        var ejemplar = await _context.Ejemplares.FindAsync(id);
        if (ejemplar == null) return NotFound();

        if (await _context.Prestamos.AnyAsync(p => p.EjemplarId == id))
            return Conflict("No se puede eliminar: el ejemplar tiene préstamos registrados.");

        _context.Ejemplares.Remove(ejemplar);
        await _context.SaveChangesAsync();

        return Ok();
    }
}