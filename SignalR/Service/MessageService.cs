using Microsoft.EntityFrameworkCore;
using SignalR.Dbcontext;
using SignalR.Models;

namespace SignalR.Service
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;
        public MessageService(AppDbContext context) => _context = context;

        public async Task<Message> SendMessageAsync(SendMessageDto dto)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = dto.ConversationId,
                SenderId = dto.SenderId,
                Content = dto.Content,
                Type = dto.Type,
                MediaUrl = dto.MediaUrl,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(message);

            var convo = await _context.Conversations.FindAsync(dto.ConversationId);
            convo.LastMessageAt = message.CreatedAt;
            convo.LastMessageContent = message.Content;
            convo.LastMessageSenderId = dto.SenderId;

            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<Message>> GetMessagesAsync(Guid conversationId, int pageNumber = 1, int pageSize = 20)
        {
            return await _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                // ❌ REMOVED: .Include(m => m.Sender) 
                // Removing this stops the 'Sender -> Messages' loop immediately.
                .AsNoTracking() // Recommended for GET requests (faster)
                .ToListAsync();
        }

        public async Task<List<Message>> SyncMessagesAsync(Guid conversationId, DateTime? lastSyncDate)
        {
            var query = _context.Messages
                .Where(m => m.ConversationId == conversationId);

            if (lastSyncDate.HasValue)
            {
                // CASE 1: The user has opened the chat before.
                // specific logic: Get only messages NEWER than what they have locally.
                query = query.Where(m => m.CreatedAt > lastSyncDate.Value)
                             .OrderBy(m => m.CreatedAt); // Chronological order (oldest to newest) to append to list
            }
            else
            {
                // CASE 2: First time install or cleared cache.
                // specific logic: Get the last 50 messages to start with.
                query = query.OrderByDescending(m => m.CreatedAt)
                             .Take(50);
            }

            var messages = await query.Include(m => m.Sender).ToListAsync();

            // If it was the initial load (Case 2), reverse them so they appear in correct order in the chat UI
            if (!lastSyncDate.HasValue)
            {
                messages.Reverse();
            }

            return messages;
        }

        public async Task<List<Message>> GetMessageHistoryAsync(Guid conversationId, DateTime? beforeDate, int pageSize = 20)
        {
            var query = _context.Messages
                .Where(m => m.ConversationId == conversationId);

            // If 'beforeDate' is provided, load older messages (Scroll Up)
            if (beforeDate.HasValue)
            {
                query = query.Where(m => m.CreatedAt < beforeDate.Value);
            }

            // Always order by CreatedAt Descending to get the "closest" messages to that date
            return await query
                .OrderByDescending(m => m.CreatedAt)
                .Take(pageSize)
                .Include(m => m.Sender)
                .ToListAsync();
        }
    }
}
