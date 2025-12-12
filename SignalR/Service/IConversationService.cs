using SignalR.Models;

namespace SignalR.Service
{
    public interface IConversationService
    {
        Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(Guid userId);
        Task<ConversationDto?> CreateOrGetConversationAsync(Guid currentUserId, Guid targetUserId);
        Task<bool> DeleteConversationAsync(Guid conversationId);
    }
}
