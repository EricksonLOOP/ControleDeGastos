using CGD.Domain.Entities;
using CGD.Domain.IRepositories;
using CGD.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace CGD.Infra.Repositories;

public class UserRepository(CGDDbContext context) : IUserRepository
{
    private readonly CGDDbContext _context = context;

    public async Task<User> AddAsync(User user)
    {
        var userCreated = _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return userCreated.Entity;
    }

    public async Task<IReadOnlyList<User>> GetPagedAsync(int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        return await _context.Users
            .AsNoTracking()
            .OrderBy(u => u.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<List<User>> GetEnrichedUsers(Guid adminGroupId)
    {
        // Escopo: retorna usuarios que participam de grupos criados pelo admin informado.
        // Include/ThenInclude carregam navegacoes usadas na camada APP sem roundtrips extras.
        return await _context.Users
            .Include(u => u.GroupMembers)
            .ThenInclude(gm => gm.Group)
            .Where(u => u.GroupMembers.Any(gm => gm.Group.CreatedBy == adminGroupId))
            .ToListAsync();
    }
}