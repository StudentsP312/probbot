namespace TelegramBot;

public class BotConfiguration
{
    public string BotToken { get; init; } = default!;
    public string RedisConnectionString { get; init; } = default!;
}
