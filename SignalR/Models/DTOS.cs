namespace SignalR.Models
{
    public class RegisterDto
    {
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class LoginDto
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }

    public class FriendRequestDto
    {
        public Guid ReceiverId { get; set; }
        public string CaseType { get; set; }
        public string CaseDescription { get; set; }
    }

    public class AcceptFriendRequestDto
    {
        public Guid RequestId { get; set; }
    }

    public class SendMessageDto
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; }
        public string Type { get; set; } // text, image, etc
        public string MediaUrl { get; set; }
        public string? ConnectionId { get; set; }
    }
    public class GetMessagesDto
    {
        public Guid ConversationId { get; set; }
        public int PageNumber { get; set; } = 1;  // 1-based
        public int PageSize { get; set; } = 20;   // Number of messages per load
    }
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
    }
    // OUTPUT: What the frontend receives
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } // Useful for Group Chats
        public bool IsGroup { get; set; }

        // Last Message Preview
        public string? LastMessageContent { get; set; }
        public DateTime LastMessageAt { get; set; }

        // List of the OTHER people in the chat (excluding the current user)
        public List<UserDto> OtherParticipants { get; set; }
    }

    // INPUT: What the frontend sends to start a chat
    public class CreateConversationDto
    {
        public Guid TargetUserId { get; set; } // For 1-on-1 chat
        // OR public List<Guid> TargetUserIds { get; set; } // If you support groups
    }
    public class PendingFriendRequestDto
    {
        public Guid RequestId { get; set; }  // Needed to accept/reject
        public Guid SenderId { get; set; }   // The ID of the person who sent it
        public string SenderName { get; set; } // The name to display in the UI
        public DateTime? SentAt { get; set; } // Optional: to show "2 mins ago"
    }
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        // Instead of the whole User object, just send what you need
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderAvatar { get; set; }
    }
}
