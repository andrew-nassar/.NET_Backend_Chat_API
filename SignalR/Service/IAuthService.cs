using SignalR.Models;

namespace SignalR.Service
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterDto dto);
        Task<User> LoginAsync(LoginDto dto);
    }
}
