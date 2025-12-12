using System.Text.Json.Serialization;

namespace SignalR.Models
{
    public class Message
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }
        [JsonIgnore]
        public Conversation Conversation { get; set; }

        public Guid SenderId { get; set; }
        public User Sender { get; set; }

        public string Type { get; set; }
        public string Content { get; set; }
        public string MediaUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
