using Microsoft.AspNetCore.SignalR;
using SignalR.Models;

namespace SignalR.Service
{
    // [Authorize] // Recommended: Use JWT Token instead of QueryString for security later
    public class ChatHub : Hub
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private readonly IConversationService _conversationService;

        public ChatHub(
            IUserService userService,
            IMessageService messageService,
            IConversationService conversationService)
        {
            _userService = userService;
            _messageService = messageService;
            _conversationService = conversationService;
        }

        public override async Task OnConnectedAsync()
        {
            var userIdString = Context.GetHttpContext()?.Request.Query["userId"].ToString();

            if (Guid.TryParse(userIdString, out var userId))
            {
                // 1. Set User Online
                await _userService.SetUserOnlineStatusAsync(userId, true);

                // 2. Create a "Personal Channel" for this user
                // This allows us to send Friend Requests or System Alerts directly to them
                await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());

                // 3. Auto-Join all their existing Conversation Groups
                // This ensures they receive messages immediately without the frontend 
                // having to manually "join" every chat room.
                var userConversations = await _conversationService.GetUserConversationsAsync(userId);
                foreach (var chat in userConversations)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, chat.Id.ToString());
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdString = Context.GetHttpContext()?.Request.Query["userId"].ToString();

            if (Guid.TryParse(userIdString, out var userId))
            {
                await _userService.SetUserOnlineStatusAsync(userId, false);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // --- MESSAGING ---

        public async Task SendMessage(SendMessageDto dto)
        {
            // 1. Save to DB
            var msg = await _messageService.SendMessageAsync(dto);

            // 2. Broadcast to the specific Conversation Group
            await Clients.Group(dto.ConversationId.ToString()).SendAsync("ReceiveMessage", msg);

            // 3. (Optional) Send a "ChatUpdated" event so the conversation list re-orders on the frontend
            await Clients.Group(dto.ConversationId.ToString()).SendAsync("UpdateConversationList", dto.ConversationId, msg);
        }

        // --- TYPING INDICATORS ---

        public async Task Typing(Guid conversationId, string userName)
        {
            // Broadcast to everyone in the chat EXCEPT the sender
            await Clients.GroupExcept(conversationId.ToString(), Context.ConnectionId)
                .SendAsync("UserTyping", conversationId, userName);
        }

        public async Task StopTyping(Guid conversationId)
        {
            await Clients.GroupExcept(conversationId.ToString(), Context.ConnectionId)
                .SendAsync("UserStoppedTyping", conversationId);
        }

        // --- READ RECEIPTS ---
        public async Task MarkMessagesAsRead(Guid conversationId, Guid userId)
        {
            // Update DB (You need to implement this in MessageService)
            // await _messageService.MarkAsReadAsync(conversationId, userId);

            // Notify others that messages were read
            await Clients.Group(conversationId.ToString()).SendAsync("MessagesRead", conversationId, userId);
        }

        // --- CONVERSATION MANAGEMENT ---

        // Call this when a user Creates a NEW chat, so they get added to the SignalR group immediately
        public async Task JoinNewConversation(Guid conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }
    }
}