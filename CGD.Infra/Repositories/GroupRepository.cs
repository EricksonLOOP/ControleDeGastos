using System.Collections.Generic;
using System.Threading.Tasks;
using CGD.Domain.Entities;
using CGD.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CGD.Infra.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly Data.CGDDbContext _context;

        public GroupRepository(Data.CGDDbContext context)
        {
            _context = context;
        }


        public async Task<Group?> GetByIdAsync(Guid id, Guid userId)
        {
            return await _context.Groups
                .Include(g => g.Members)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.Id == id && g.CreatedBy == userId);
        }

        public async Task<IEnumerable<Group>> GetAllAsync(Guid userId)
        {
            // Visibilidade de grupos: usuario ve grupos que criou ou onde e membro.
            return await _context.Groups
                .Include(g => g.Members)
                .ThenInclude(m => m.User)
                .Where(g => g.CreatedBy == userId ||
                            g.Members.Any(m => m.UserId == userId))
                .ToListAsync();
        }

        public async Task AddAsync(Group group)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Group group)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }
        }
    }
}
