using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalR.Models;
using SignalR.Service;

namespace SignalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) => _userService = userService;

        [HttpGet("{id}/friends")]
        public async Task<IActionResult> GetFriends(Guid id)
        {
            var friends = await _userService.GetFriendsAsync(id);

            // Convert the Entities to DTOs before returning
            var friendDtos = friends.Select(f => new UserDto
            {
                Id = f.Id,
                Username = f.Name ?? "Unknown", // Handle potential nulls
            });

            return Ok(friendDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
        // [HttpGet("all")] -> Change this to encompass the specific logic
        [HttpGet("{currentUserId}/suggestions")]
        public async Task<IActionResult> GetUsersToFriended(
            Guid currentUserId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _userService.GetUsersNotFriendsAsync(currentUserId, pageNumber, pageSize);
            return Ok(result);
        }
    }
}
