// Repositories/LibroRepository.cs
using Microsoft.EntityFrameworkCore;
using TarijaReadApp.Data;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;

namespace TarijaReadApp.Repositories;

public class LibroRepository : Repository<Libro>, ILibroRepository
{
    public LibroRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<IEnumerable<Libro>> GetAllAsync() =>
        await _context.Libros
            .Include(l => l.Categoria)
            .AsNoTracking()
            .ToListAsync();

    public override async Task<Libro?> GetByIdAsync(int id) =>
        await _context.Libros
            .Include(l => l.Categoria)
            .FirstOrDefaultAsync(l => l.Id == id);
}