using SignalR.Models;

namespace SignalR.Service
{
    public interface IMessageService
    {
        Task<Message> SendMessageAsync(SendMessageDto dto);
        Task<List<Message>> GetMessagesAsync(Guid conversationId, int pageNumber = 1, int pageSize = 20);
        // Add this line:
        Task<List<Message>> SyncMessagesAsync(Guid conversationId, DateTime? lastSyncDate);
        Task<List<Message>> GetMessageHistoryAsync(Guid conversationId, DateTime? beforeDate, int pageSize = 20);
    }
}
