using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;

namespace TarijaReadApp.Controllers;

[ApiController]
[Route("api/Prestamos")]

public class PrestamoApiController : ControllerBase
{
    private readonly IPrestamoRepository _repository;

    public PrestamoApiController(IPrestamoRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("GetPrestamos")]
public async Task<IActionResult> GetPrestamos()
{
    var prestamos = await _repository.GetAllAsync();

    var resultado = prestamos.Select(p => new
    {
        id = p.Id,
        fechaSalida = p.FechaSalida,
        fechaLimite = p.FechaLimite,
        fechaDevolucion = p.FechaDevolucion,

        socio = p.Socio == null ? null : new
        {
            nombre = p.Socio.Nombre
        },

        ejemplar = p.Ejemplar == null ? null : new
        {
            codigo = p.Ejemplar.Codigo
        }
    });

    return Ok(resultado);
}   
[HttpPost("CreateMulta")]
    public async Task<IActionResult> CreatePrestamo([FromBody] Prestamo prestamo)
    {
        if (prestamo == null)
        {
            return BadRequest("El prestamo no puede ser nulo.");
        }

        await _repository.AddAsync(prestamo);
        await _repository.SaveChangesAsync();
        return Ok(prestamo);
    }
}