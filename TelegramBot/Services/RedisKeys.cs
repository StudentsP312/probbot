namespace TelegramBot.Services;

public static class CacheKeys
{
    private const string Base = "probbot";

    public static string UserProfile(long userId) => $"{Base}:user:{userId}:profile";

    public static string UserGameState(long userId) => $"{Base}:user:{userId}:game_state";

    public static string GameSession(string gameType, Guid sessionId) =>
        $"{Base}:game:{gameType}:{sessionId}";
}
