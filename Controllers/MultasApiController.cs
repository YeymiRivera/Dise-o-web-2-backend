using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;
using TarijaReadApp.Data;
using Microsoft.EntityFrameworkCore;

namespace TarijaReadApp.Controllers;

[ApiController]
[Route("api/Multas")]

public class MultasApiController : ControllerBase
{
    private readonly IMultaRepository _repository;
    private readonly ApplicationDbContext _context;

    public MultasApiController(IMultaRepository repository,ApplicationDbContext context)
    {
        _repository = repository;
        _context=context;
    }

    [HttpGet("GetMultas")]
    public async Task<IActionResult> GetMultas()
    {
        var multas = await _repository.GetAllAsync();

        var resultado = multas.Select(m => new
        {
            id = m.Id,
            monto = m.Monto,
            pagoRealizado = m.PagoRealizado,
            prestamoId = m.PrestamoId,
            prestamo = m.Prestamo == null ? null : new
            {
                id = m.Prestamo.Id,
                socio = m.Prestamo.Socio == null ? null : new
                {
                    nombre = m.Prestamo.Socio.Nombre
                }
            }
        });

        return Ok(resultado);
    }
[HttpPost("CreateMulta")]
    public async Task<IActionResult> CreateMulta([FromBody] Multa multa)
    {
        if (multa == null)
        {
            return BadRequest("La multa no puede ser nula.");
        }
         bool yaExiste = await _context.Multas.AnyAsync(m => m.PrestamoId == multa.PrestamoId);

    if (yaExiste)
    {
        return Conflict(
            "Este préstamo ya tiene una multa registrada.");
    }
   
        await _repository.AddAsync(multa);
        await _repository.SaveChangesAsync();
        return Ok(multa);
    }
    [HttpPut("EditMultas")]
    public async Task<IActionResult>EditarMulta([FromBody] Multa multa)
    {
    var multaeditada=await (from m in _context.Multas 
    where m.PrestamoId==multa.PrestamoId
    select m).FirstOrDefaultAsync();
    if(multaeditada==null){return NotFound();}
    multaeditada.Monto=multa.Monto;
    multaeditada.PagoRealizado=multa.PagoRealizado;
    await _repository.SaveChangesAsync();
    return Ok();}
    [HttpDelete("EliminarMulta")]
    public async Task<IActionResult> EliminarMulta( int id)
    {
        var multaeliminada= await (from m in _context.Multas
        where m.PrestamoId==id
        select m).FirstOrDefaultAsync();
        if (multaeliminada == null)
        {
            return NotFound();
        }
        _context.Multas.Remove(multaeliminada);
        await _context.SaveChangesAsync();
        return Ok();
    }
}