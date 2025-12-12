namespace SignalR.Models
{
    public class Conversation
    {
        public Guid Id { get; set; }

        public DateTime LastMessageAt { get; set; }
        public string? LastMessageContent { get; set; }
        public Guid? LastMessageSenderId { get; set; }

        public ICollection<Message> Messages { get; set; }
        public ICollection<ConversationParticipant> Participants { get; set; }
    }
}
