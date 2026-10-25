using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TarijaReadApp.Data;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;
using TarijaReadApp.ViewModels;

namespace TarijaReadApp.Controllers.Api;

[ApiController]
[Route("api/Categorias")]
public class CategoriasApiController : ControllerBase
{
    private readonly IRepository<Categoria> _repository;
    private readonly ApplicationDbContext _context;

    public CategoriasApiController(IRepository<Categoria> repository, ApplicationDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    // GET: api/Categorias/GetCategorias
    [HttpGet("GetCategorias")]
    public async Task<IActionResult> GetCategorias()
    {
        var categorias = await _repository.GetAllAsync();

        var resultado = categorias
            .OrderBy(c => c.Nombre)
            .Select(c => new { id = c.Id, nombre = c.Nombre });

        return Ok(resultado);
    }

    // GET: api/Categorias/GetCategoria/5
    [HttpGet("GetCategoria/{id:int}")]
    public async Task<IActionResult> GetCategoria(int id)
    {
        var categoria = await _repository.GetByIdAsync(id);
        if (categoria == null) return NotFound();

        return Ok(new { id = categoria.Id, nombre = categoria.Nombre });
    }

    // POST: api/Categorias/CreateCategoria
    [HttpPost("CreateCategoria")]
    public async Task<IActionResult> CreateCategoria([FromBody] CategoriaInput input)
    {
        var nombre = input.Nombre.Trim();

        bool yaExiste = await _context.Categorias
            .AnyAsync(c => c.Nombre.ToLower() == nombre.ToLower());
        if (yaExiste)
            return Conflict("Ya existe una categoría con ese nombre.");

        var categoria = new Categoria { Nombre = nombre };
        await _repository.AddAsync(categoria);
        await _repository.SaveChangesAsync();

        return Ok(new { id = categoria.Id, nombre = categoria.Nombre });
    }

    // PUT: api/Categorias/EditCategoria/5
    [HttpPut("EditCategoria/{id:int}")]
    public async Task<IActionResult> EditCategoria(int id, [FromBody] CategoriaInput input)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null) return NotFound();

        var nombre = input.Nombre.Trim();

        bool yaExiste = await _context.Categorias
            .AnyAsync(c => c.Id != id && c.Nombre.ToLower() == nombre.ToLower());
        if (yaExiste)
            return Conflict("Ya existe otra categoría con ese nombre.");

        categoria.Nombre = nombre;
        await _context.SaveChangesAsync();

        return Ok(new { id = categoria.Id, nombre = categoria.Nombre });
    }

    // DELETE: api/Categorias/EliminarCategoria/5
    [HttpDelete("EliminarCategoria/{id:int}")]
    public async Task<IActionResult> EliminarCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null) return NotFound();

        bool tieneLibros = await _context.Libros.AnyAsync(l => l.CategoriaId == id);
        if (tieneLibros)
            return Conflict("No se puede eliminar: la categoría tiene libros asociados.");

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return Ok();
    }
}