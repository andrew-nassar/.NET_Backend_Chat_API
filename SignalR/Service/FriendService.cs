using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.Dbcontext;
using SignalR.Models;

namespace SignalR.Service
{
    public class FriendService : IFriendService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext; // Inject this
        private readonly IConversationService _conversationService;

        public FriendService(AppDbContext context, IHubContext<ChatHub> hubContext, IConversationService conversationService)
        {
            _context = context;
            _hubContext = hubContext;
            _conversationService = conversationService;
        }

        public async Task SendFriendRequestAsync(Guid senderId, FriendRequestDto dto)
        {
            var request = new FriendRequest
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                CaseType = dto.CaseType,
                CaseDescription = dto.CaseDescription,
                Status = FriendRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            _context.FriendRequests.Add(request);
            await _context.SaveChangesAsync();

            // REAL-TIME NOTIFICATION
            // Send to the Receiver's Personal Channel (receiverId.ToString())
            await _hubContext.Clients.Group(dto.ReceiverId.ToString())
                .SendAsync("ReceiveFriendRequest", new
                {
                    SenderId = senderId,
                    SenderName = "Name From DB"
                });
        }

        public async Task AcceptFriendRequestAsync(Guid receiverId, AcceptFriendRequestDto dto)
        {
            // ... (كودك القديم لجلب الطلب) ...
            var request = await _context.FriendRequests
                .Include(fr => fr.Sender)
                    .ThenInclude(u => u.Friends)
                .Include(fr => fr.Receiver)
                    .ThenInclude(u => u.Friends)
                .FirstOrDefaultAsync(fr => fr.Id == dto.RequestId && fr.ReceiverId == receiverId);

            if (request == null) return;

            // ... (كودك القديم لإضافة الأصدقاء) ...
            if (!request.Sender.Friends.Contains(request.Receiver))
            {
                request.Sender.Friends.Add(request.Receiver);
            }
            if (!request.Receiver.Friends.Contains(request.Sender))
            {
                request.Receiver.Friends.Add(request.Sender);
            }

            request.Status = FriendRequestStatus.Accepted;

            // حفظ التغييرات الخاصة بالصداقة أولاً
            await _context.SaveChangesAsync();

            // 3️⃣ 🔥 اللمسة السحرية: إنشاء المحادثة تلقائياً 🔥
            // بناخد الـ SenderId من الطلب نفسه، والـ ReceiverId هو المستقبل
            await _conversationService.CreateOrGetConversationAsync(request.SenderId, request.ReceiverId);
        }

        public async Task<IEnumerable<PendingFriendRequestDto>> GetPendingRequestsAsync(Guid userId)
        {
            // Assuming you are using Entity Framework Core
            var requests = await _context.FriendRequests
                .Include(r => r.Sender) // Join with User table to get names
                .Where(r => r.ReceiverId == userId && r.Status == FriendRequestStatus.Pending) // Filter by Receiver & Status
                .Select(r => new PendingFriendRequestDto
                {
                    RequestId = r.Id,
                    SenderId = r.SenderId,
                    SenderName = r.Sender.Name, // Or r.Sender.FullName
                    SentAt = r.CreatedAt
                })
                .ToListAsync();

            return requests;
        }
    }
}
