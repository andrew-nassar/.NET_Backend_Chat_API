using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalR.Models;
using SignalR.Service;

namespace SignalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService _friendService;
        public FriendController(IFriendService friendService) => _friendService = friendService;

        [HttpPost("send")]
        public async Task<IActionResult> SendFriendRequest(Guid senderId, FriendRequestDto dto)
        {
            await _friendService.SendFriendRequestAsync(senderId, dto);
            return Ok();
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptFriendRequest(Guid receiverId, AcceptFriendRequestDto dto)
        {
            await _friendService.AcceptFriendRequestAsync(receiverId, dto);
            return Ok();
        }
        // GET: api/friend/requests/{userId}
        [HttpGet("requests/{userId}")]
        public async Task<IActionResult> GetPendingRequests(Guid userId)
        {
            var requests = await _friendService.GetPendingRequestsAsync(userId);

            // Returns 200 OK with the list of requests
            return Ok(requests);
        }
    }
}
