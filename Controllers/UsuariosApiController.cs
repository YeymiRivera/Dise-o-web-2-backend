using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;
using TarijaReadApp.Data;
using Microsoft.EntityFrameworkCore;

namespace TarijaReadApp.Controllers;

[ApiController]
[Route("api/Usuarios")]

public class UsuariosApiController : ControllerBase
{
    private readonly IMultaRepository _repository;
    private readonly ApplicationDbContext _context;

    public UsuariosApiController(IMultaRepository repository,ApplicationDbContext context)
    {
        _repository = repository;
        _context=context;
    }
    
    [HttpGet("GetUsuarios")]
    public async Task<IActionResult> GetUsuarios()
    {
        var usuarios= await _context.Usuarios.ToListAsync();
       var resultado = usuarios.Select(u => new
    {
        id = u.Id,
        nombre = u.Nombre,
        email = u.Email,
        rol = u.Rol.ToString(),
        prestamos = u.Prestamos
    });
        return Ok(resultado);
    }
    [HttpPost("PostUsuario")]
    public async Task<IActionResult> PostUsuarios([FromBody]Usuario usuario)
    {
        if(usuario==null){return NotFound();}
        var yaexiste= await (from u in _context.Usuarios where u.Email==usuario.Email select u).FirstOrDefaultAsync();
        if (yaexiste != null)
        {
            return NotFound("Ya existe un usuario con ese correo");
        }
        await _context.AddAsync(usuario);
        await _context.SaveChangesAsync();
        return Ok();
    }
    [HttpPut("PutUsuario")]
    public async Task<IActionResult> PutUsuarios([FromBody] Usuario usuario)
    {
        if (usuario == null)
        {
            return NotFound("Usuario no valido");
        }
        var user=await (from u in _context.Usuarios
        where usuario.Id==u.Id select u ).FirstOrDefaultAsync();
        if (user == null)
        {
            return NotFound("Usuario no existente");
        }
          bool correoExiste = await _context.Usuarios
        .AnyAsync(u => u.Email == usuario.Email && u.Id != usuario.Id);

    if (correoExiste)
    {
        return Conflict("El correo ya está registrado.");
    }
        user.Email=usuario.Email;
        user.Nombre=usuario.Nombre;
        user.Rol=usuario.Rol;
        await _context.SaveChangesAsync();
        return Ok();
    }
    [HttpDelete("DeleteUsuario")]
    public async Task<IActionResult> DeleteUsuarios(int id) 
    {
        var usuario=await (from u in _context.Usuarios where u.Id==id select u).FirstOrDefaultAsync();
        if (usuario == null)
        {
            return NotFound("Usuario no existe");
        }
        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        return Ok();
    }
    
}
