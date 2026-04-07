# Real-Time Chat & Social Connect (SignalR) 🚀

A robust real-time communication system built with **ASP.NET Core Web API** and **SignalR**, following **Clean Architecture** principles and a service-oriented design.

## 🌟 Key Features
*   **Instant Messaging:** Real-time text exchange using SignalR Hubs.
*   **Social System:** Comprehensive friend request workflow (Send, Receive, Accept/Reject).
*   **Dynamic Conversations:** Supports private and group chat structures via `ConversationParticipants`.
*   **Advanced Data Modeling:** Complex many-to-many relationships and data integrity using EF Core Fluent API.
*   **Service-Oriented Architecture:** Decoupled business logic using Interfaces and Services.

## 🛠 Tech Stack
*   **Framework:** .NET 8 / ASP.NET Core Web API
*   **Real-Time:** Microsoft SignalR
*   **ORM:** Entity Framework Core (SQL Server)
*   **Design Patterns:** Service Pattern, Interface Segregation, Dependency Injection.

## 📂 Project Structure
*   `Dbcontext/`: EF Core configurations and Fluent API relationship mapping.
*   `Services/`: Business logic layer (Auth, Chat, Friendships, Messages).
*   `Models/`: Database Entities (User, Conversation, Message, FriendRequest).
*   `Hubs/`: SignalR Hubs for managing WebSocket connections.

## 💾 Database Schema Highlights
The system handles complex data relations:
- **Composite Keys:** Used for `ConversationParticipant` to ensure data uniqueness.
- **Indexing:** Unique indexes on `FriendRequest` to prevent duplicate requests.
- **Delete Behaviors:** Strategic use of `Restrict` vs `Cascade` to maintain data history.

## 🚀 Getting Started
1. Clone the repo.
2. Update the connection string in `appsettings.json`.
3. Run `Update-Database` in Package Manager Console.
4. Hit `F5` to run the API and explore via Swagger.
