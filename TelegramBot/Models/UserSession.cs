namespace TelegramBot.Models;

public class UserSession
{
    public long UserId { get; set; }
    public string CurrentGame { get; set; } = "None";
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string> Metadata { get; set; } = new();
}
