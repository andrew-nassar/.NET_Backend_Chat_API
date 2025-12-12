using SignalR.Models;

namespace SignalR.Service
{
    public interface IFriendService
    {
        Task SendFriendRequestAsync(Guid senderId, FriendRequestDto dto);
        Task AcceptFriendRequestAsync(Guid receiverId, AcceptFriendRequestDto dto);
        // --- ADD THIS LINE ---
        Task<IEnumerable<PendingFriendRequestDto>> GetPendingRequestsAsync(Guid userId);
    }
}
