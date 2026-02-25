using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CGD.APP.Services.Groups;
using CGD.APP.DTOs.Group;
using System.Collections.Generic;

namespace CGD.API.Controllers
{
    [ApiController]
    [Route("groups")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetAll()
        {
            var userId = GetUserId();
            var groups = await _groupService.GetAllAsync(userId);
            return Ok(groups);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetById(Guid id)
        {
            var userId = GetUserId();
            var group = await _groupService.GetByIdAsync(id, userId);
            if (group == null) return NotFound();
            return Ok(group);
        }


        [HttpPost]
        public async Task<ActionResult<GroupDto>> Create([FromBody] GroupCreateDto dto)
        {
            var userId = GetUserId();
            var group = await _groupService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<GroupDto>> Update(Guid id, [FromBody] UpdateGroupDto dto)
        {
            var userId = GetUserId();
            var group = await _groupService.UpdateAsync(id, dto.Name, userId);
            if (group == null) return NotFound();
            return Ok(group);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _groupService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{groupId}/users/{userId}")]
        public async Task<ActionResult> AddUserToGroup(Guid groupId, Guid userId)
        {
            var groupAdmin = GetUserId();
            await _groupService.AddUserToGroupAsync(groupId, groupAdmin, userId);
            return NoContent();
        }

        [HttpDelete("{groupId}/users/{userId}")]
        public async Task<ActionResult> RemoveUserFromGroup(Guid groupId, Guid userId)
        {
            await _groupService.RemoveUserFromGroupAsync(groupId, userId);
            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroupsByUser(Guid userId)
        {
            var groups = await _groupService.GetGroupsByUserIdAsync(userId);
            return Ok(groups);
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier || c.Type == "sub");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException();
            return userId;
        }
    }
}
