using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TarijaReadApp.Data;
using TarijaReadApp.Models;
using TarijaReadApp.ViewModels;

namespace TarijaReadApp.Controllers.Api;

[ApiController]
[Route("api/Socios")]
public class SociosApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SociosApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("GetSocios")]
    public async Task<IActionResult> GetSocios()
    {
        var socios = await _context.Socios
            .AsNoTracking()
            .OrderBy(s => s.Nombre)
            .Select(s => new { id = s.Id, nombre = s.Nombre, ci = s.CI, email = s.Email, telefono = s.Telefono })
            .ToListAsync();

        return Ok(socios);
    }

    [HttpGet("GetSocio/{id:int}")]
    public async Task<IActionResult> GetSocio(int id)
    {
        var s = await _context.Socios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return NotFound();

        return Ok(new { id = s.Id, nombre = s.Nombre, ci = s.CI, email = s.Email, telefono = s.Telefono });
    }

    [HttpPost("CreateSocio")]
    public async Task<IActionResult> CreateSocio([FromBody] SocioInput input)
    {
        var ci = input.CI.Trim();

        bool ciExiste = await _context.Socios.AnyAsync(s => s.CI == ci);
        if (ciExiste)
            return Conflict("Ya existe un socio con ese CI.");

        var socio = new Socio
        {
            Nombre = input.Nombre.Trim(),
            CI = ci,
            Email = string.IsNullOrWhiteSpace(input.Email) ? null : input.Email.Trim(),
            Telefono = string.IsNullOrWhiteSpace(input.Telefono) ? null : input.Telefono.Trim()
        };

        _context.Socios.Add(socio);
        await _context.SaveChangesAsync();

        return Ok(new { id = socio.Id, nombre = socio.Nombre, ci = socio.CI, email = socio.Email, telefono = socio.Telefono });
    }

    [HttpPut("EditSocio/{id:int}")]
    public async Task<IActionResult> EditSocio(int id, [FromBody] SocioInput input)
    {
        var socio = await _context.Socios.FindAsync(id);
        if (socio == null) return NotFound();

        var ci = input.CI.Trim();

        bool ciExiste = await _context.Socios.AnyAsync(s => s.Id != id && s.CI == ci);
        if (ciExiste)
            return Conflict("Ya existe otro socio con ese CI.");

        socio.Nombre = input.Nombre.Trim();
        socio.CI = ci;
        socio.Email = string.IsNullOrWhiteSpace(input.Email) ? null : input.Email.Trim();
        socio.Telefono = string.IsNullOrWhiteSpace(input.Telefono) ? null : input.Telefono.Trim();

        await _context.SaveChangesAsync();

        return Ok(new { id = socio.Id, nombre = socio.Nombre, ci = socio.CI, email = socio.Email, telefono = socio.Telefono });
    }

    [HttpDelete("EliminarSocio/{id:int}")]
    public async Task<IActionResult> EliminarSocio(int id)
    {
        var socio = await _context.Socios.FindAsync(id);
        if (socio == null) return NotFound();

        bool tienePrestamos = await _context.Prestamos.AnyAsync(p => p.SocioId == id);
        if (tienePrestamos)
            return Conflict("No se puede eliminar: el socio tiene préstamos registrados.");

        _context.Socios.Remove(socio);
        await _context.SaveChangesAsync();

        return Ok();
    }
}