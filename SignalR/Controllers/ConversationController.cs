using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalR.Models;
using SignalR.Service;

namespace SignalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserConversations(Guid userId)
        {
            // You can assume if list is empty, it's just a user with no chats.
            // But if you strictly want to check if UserID is valid, you'd do that in service.
            var chats = await _conversationService.GetUserConversationsAsync(userId);
            return Ok(chats);
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartConversation(Guid currentUserId, [FromBody] CreateConversationDto dto)
        {
            if (currentUserId == dto.TargetUserId)
            {
                return BadRequest("You cannot start a conversation with yourself.");
            }

            var chat = await _conversationService.CreateOrGetConversationAsync(currentUserId, dto.TargetUserId);

            if (chat == null)
            {
                return BadRequest("Target user not found or could not create conversation.");
            }

            return Ok(chat);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConversation(Guid id)
        {
            var isDeleted = await _conversationService.DeleteConversationAsync(id);

            if (!isDeleted)
            {
                return NotFound(new { message = $"Conversation with ID {id} not found." });
            }

            // Return 204 No Content (Standard for Deletes) or 200 OK
            return Ok(new { message = "Conversation deleted successfully." });
        }
    }
}