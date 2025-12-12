using SignalR.Models;

namespace SignalR.Service
{
    public interface IUserService
    {
        // Rename/Update this method to reflect the logic
        Task<PagedResult<UserDto>> GetUsersNotFriendsAsync(Guid currentUserId, int pageNumber, int pageSize);
        Task<User> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<User>> GetFriendsAsync(Guid userId);
        Task SetUserOnlineStatusAsync(Guid userId, bool isOnline);
    }
}
