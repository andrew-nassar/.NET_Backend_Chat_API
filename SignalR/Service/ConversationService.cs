using Microsoft.EntityFrameworkCore;
using SignalR.Dbcontext;
using SignalR.Models;

namespace SignalR.Service
{
    public class ConversationService : IConversationService
    {
        private readonly AppDbContext _context;

        public ConversationService(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET ALL CONVERSATIONS
        public async Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(Guid userId)
        {
            // Optional: Check if user exists first
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                // You can throw a custom exception or return null/empty based on your preference
                // For now, an empty list is technically correct (user has 0 chats)
                return new List<ConversationDto>();
            }

            var conversations = await _context.Conversations
                .Where(c => c.Participants.Any(p => p.UserId == userId))
                .Include(c => c.Participants)
                .ThenInclude(p => p.User)
                .OrderByDescending(c => c.LastMessageAt)
                .Select(c => new ConversationDto
                {
                    Id = c.Id,
                    LastMessageContent = c.LastMessageContent,
                    LastMessageAt = c.LastMessageAt,
                    OtherParticipants = c.Participants
                        .Where(p => p.UserId != userId)
                        .Select(p => new UserDto
                        {
                            Id = p.User.Id,
                            Username = p.User.Name, // Ensure this matches your User entity property
                        }).ToList()
                })
                .ToListAsync();

            return conversations;
        }

        // 2. CREATE OR GET
        public async Task<ConversationDto?> CreateOrGetConversationAsync(Guid currentUserId, Guid targetUserId)
        {
            // VALIDATION: Check if target user actually exists
            var targetUserExists = await _context.Users.AnyAsync(u => u.Id == targetUserId);
            if (!targetUserExists)
            {
                return null; // Indicates failure
            }

            // A. Check for existing
            var existingConversation = await _context.Conversations
                .Where(c => c.Participants.Count == 2 &&
                            c.Participants.Any(p => p.UserId == currentUserId) &&
                            c.Participants.Any(p => p.UserId == targetUserId))
                .FirstOrDefaultAsync();

            if (existingConversation != null)
            {
                return await GetConversationDto(existingConversation.Id, currentUserId);
            }

            // B. Create New
            var newConversation = new Conversation
            {
                Id = Guid.NewGuid(),
                LastMessageAt = DateTime.UtcNow,
                Participants = new List<ConversationParticipant>
            {
                new ConversationParticipant { UserId = currentUserId },
                new ConversationParticipant { UserId = targetUserId }
            }
            };

            _context.Conversations.Add(newConversation);
            await _context.SaveChangesAsync();

            return await GetConversationDto(newConversation.Id, currentUserId);
        }

        // 3. DELETE (Returns bool now)
        public async Task<bool> DeleteConversationAsync(Guid conversationId)
        {
            var conversation = await _context.Conversations.FindAsync(conversationId);
            if (conversation == null)
            {
                return false; // Not Found
            }

            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync();
            return true; // Success
        }

        // Helper
        private async Task<ConversationDto?> GetConversationDto(Guid conversationId, Guid currentUserId)
        {
            // Changed FirstAsync to FirstOrDefaultAsync to avoid crashes
            return await _context.Conversations
                .Where(c => c.Id == conversationId)
                .Select(c => new ConversationDto
                {
                    Id = c.Id,
                    LastMessageContent = c.LastMessageContent,
                    LastMessageAt = c.LastMessageAt,
                    OtherParticipants = c.Participants
                        .Where(p => p.UserId != currentUserId)
                        .Select(p => new UserDto
                        {
                            Id = p.User.Id,
                            Username = p.User.Name,
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }

    }
}
