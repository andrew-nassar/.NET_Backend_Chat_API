using Microsoft.EntityFrameworkCore;
using SignalR.Dbcontext;
using SignalR.Models;

namespace SignalR.Service
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context) => _context = context;

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.Include(u => u.Friends).FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<User>> GetFriendsAsync(Guid userId)
        {
            var user = await _context.Users.Include(u => u.Friends).FirstOrDefaultAsync(u => u.Id == userId);
            return user?.Friends ?? Enumerable.Empty<User>();
        }

        public async Task SetUserOnlineStatusAsync(Guid userId, bool isOnline)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsOnline = isOnline;
                user.LastSeen = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

        }
        // REVISED LOGIC: Exclude Friends AND Pending Requests
        public async Task<PagedResult<UserDto>> GetUsersNotFriendsAsync(Guid currentUserId, int pageNumber, int pageSize)
        {
            // 1. Get IDs of existing connections (Friends + Pending)
            var existingConnectionIds = await _context.FriendRequests
                .Where(fr => (fr.SenderId == currentUserId || fr.ReceiverId == currentUserId) && fr.Status != FriendRequestStatus.Rejected)
                .Select(fr => fr.SenderId == currentUserId ? fr.ReceiverId : fr.SenderId)
                .ToListAsync();

            // 2. Build the "Base Query" (Before sorting or paging)
            // We use AsQueryable to build the logic without executing it yet
            var baseQuery = _context.Users
                .Where(u => u.Id != currentUserId && !existingConnectionIds.Contains(u.Id));

            // 3. EXECUTE COUNT: Get total available matches
            // This allows the frontend to know if there are 50 users or 5000 users waiting.
            var totalCount = await baseQuery.CountAsync();

            // 4. EXECUTE FETCH: Get the specific page of data
            var items = await baseQuery
                .OrderBy(u => u.Name) // Sorting is mandatory for pagination
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Name,
                })
                .ToListAsync();

            // 5. Wrap it all up
            return new PagedResult<UserDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
