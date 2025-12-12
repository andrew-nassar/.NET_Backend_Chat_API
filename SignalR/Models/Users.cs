using Microsoft.VisualBasic;

namespace SignalR.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string? ProfilePic { get; set; }
        public string PasswordHash { get; set; }

        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; }

        public ICollection<ConversationParticipant> Conversations { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<Message> Messages { get; set; }

        // Friend system
        public ICollection<FriendRequest> SentFriendRequests { get; set; }
        public ICollection<FriendRequest> ReceivedFriendRequests { get; set; }
        public ICollection<User> Friends { get; set; } // Only accepted friends
    }
}
