using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Abstract;
using TelegramBot.Models;

namespace TelegramBot.Services.Games;

public class PokerGameService(ITelegramBotClient bot, ICacheService cache) : IGameService
{
    public const string GameName = "Poker";
    public const string BaseKey = "poker_";

    public static class Keys
    {
        public const string GameState = BaseKey + "gameState";
    }

    public async Task StartGameAsync(long chatId, CancellationToken ct)
    {
        var session = new UserSession
        {
            UserId = chatId,
            CurrentGame = GameName,
            LastActivity = DateTime.UtcNow,
        };

        await cache.SetAsync(CacheKeys.UserGameState(chatId), session);

        await bot.SendMessage(
            chatId,
            "Welcome to Poker! Your session has been initialized in Redis.",
            cancellationToken: ct
        );
    }

    public async Task HandleCallbackAsync(CallbackQuery callbackQuery, CancellationToken ct)
    {
        await bot.AnswerCallbackQuery(
            callbackQuery.Id,
            "Poker interaction...",
            cancellationToken: ct
        );
    }
}
