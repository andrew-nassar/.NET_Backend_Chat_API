namespace SignalR.Models
{
    public enum FriendRequestStatus
    {
        Pending,
        Accepted,
        Rejected
    }

    public class FriendRequest
    {
        public Guid Id { get; set; }

        // User who sends the request
        public Guid SenderId { get; set; }
        public User Sender { get; set; }

        // User who receives the request
        public Guid ReceiverId { get; set; }
        public User Receiver { get; set; }

        // Case info
        public string CaseType { get; set; }
        public string CaseDescription { get; set; }

        // Request status
        public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
