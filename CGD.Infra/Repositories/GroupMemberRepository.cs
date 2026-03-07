using System.Collections.Generic;
using System.Threading.Tasks;
using CGD.Domain.Entities;
using CGD.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CGD.Infra.Repositories
{
    public class GroupMemberRepository : IGroupMemberRepository
    {
        private readonly Data.CGDDbContext _context;

        public GroupMemberRepository(Data.CGDDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GroupMember>> GetByGroupIdAsync(Guid groupId)
        {
            return await _context.GroupMembers
                .Include(gm => gm.User)
                .Where(gm => gm.GroupId == groupId)
                .ToListAsync();
        }

        public async Task<IEnumerable<GroupMember>> GetByUserIdAsync(Guid userId)
        {
            return await _context.GroupMembers
                .Include(gm => gm.Group)
                .Where(gm => gm.UserId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(GroupMember groupMember)
        {
            _context.GroupMembers.Add(groupMember);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid groupId, Guid userId)
        {
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);
            if (member != null)
            {
                _context.GroupMembers.Remove(member);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyList<GroupMember>> GetAllByGroupAdminId(Guid userId)
        {
            // Escopo: membros dos grupos cujo criador e o admin informado.
            return await _context.GroupMembers
                .Include(gm => gm.User)
                .Include(gm => gm.Group)
                .Where(gm => gm.Group.CreatedBy == userId)
                .ToListAsync();
        }
    }
}
