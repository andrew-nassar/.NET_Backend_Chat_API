using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR.Models;
using SignalR.Service;

namespace SignalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService, IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(SendMessageDto dto)
        {
            var message = await _messageService.SendMessageAsync(dto);
            // 2. Broadcast to conversation group (ensure group name matches what clients joined)
            if (!string.IsNullOrEmpty(dto.ConnectionId))
            {
                // ✅ Send to everyone EXCEPT the sender
                await _hubContext.Clients.GroupExcept(dto.ConversationId.ToString(), new[] { dto.ConnectionId })
                    .SendAsync("ReceiveMessage", message);

                // Optional: Update list for everyone EXCEPT sender
                await _hubContext.Clients.GroupExcept(dto.ConversationId.ToString(), new[] { dto.ConnectionId })
                    .SendAsync("UpdateConversationList", dto.ConversationId, message);
            }
            else
            {
                // Fallback: If no ConnectionId was provided, send to everyone (standard behavior)
                await _hubContext.Clients.Group(dto.ConversationId.ToString())
                    .SendAsync("ReceiveMessage", message);

                await _hubContext.Clients.Group(dto.ConversationId.ToString())
                    .SendAsync("UpdateConversationList", dto.ConversationId, message);
            }
            return Ok(message);
        }
        [HttpGet("{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(Guid conversationId, int pageNumber = 1, int pageSize = 20)
        {
            var messages = await _messageService.GetMessagesAsync(conversationId, pageNumber, pageSize);
            return Ok(messages);
        }
        [HttpGet("{conversationId}/sync")]
        public async Task<IActionResult> SyncMessages(Guid conversationId, [FromQuery] DateTime? lastMessageDate)
        {
            // The client sends 'lastMessageDate' (e.g., "2023-10-27T10:00:00Z")
            // If client sends nothing, lastMessageDate is null.
            var messages = await _messageService.SyncMessagesAsync(conversationId, lastMessageDate);
            return Ok(messages);
        }
        [HttpGet("{conversationId}/history")]
        public async Task<IActionResult> GetHistory(Guid conversationId, [FromQuery] DateTime? beforeDate)
        {
            // 1. If User just reinstalled app -> beforeDate is null -> Returns LATEST 20 messages.
            // 2. If User scrolls up -> beforeDate is the date of the top message -> Returns OLDER 20 messages.
            var messages = await _messageService.GetMessageHistoryAsync(conversationId, beforeDate);

            // Reverse the list so they appear chronologically (Oldest -> Newest) in the chat UI
            messages.Reverse();

            return Ok(messages);
        }
    }
}
