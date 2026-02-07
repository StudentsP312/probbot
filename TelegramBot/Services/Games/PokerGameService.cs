using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Abstract;
using TelegramBot.Games.Poker;
using TelegramBot.Models;

namespace TelegramBot.Services.Games;

public class PokerGameService(
    ITelegramBotClient bot,
    ICacheService cache,
    IGameRepository repository
) : IGameService
{
    public string GameName => "Poker";
    private const string BaseKey = "poker_";

    public async Task StartGameAsync(long chatId, CancellationToken ct)
    {
        var userSession = new UserSession
        {
            UserId = chatId,
            CurrentGame = GameName,
            LastActivity = DateTime.UtcNow,
        };
        await cache.SetAsync(CacheKeys.UserGameState(chatId), userSession);

        var pokerSession = new PokerSession();
        PokerGameLogic.StartGame(pokerSession);

        var stateJson = JsonSerializer.Serialize(pokerSession);

        await cache.SetAsync($"{BaseKey}{chatId}", pokerSession);

        await repository.CreateGameAsync(chatId, GameName, stateJson);

        await bot.SendMessage(
            chatId,
            $"Welcome to Poker! Hand: {string.Join(", ", pokerSession.PlayerHand)}",
            cancellationToken: ct
        );
    }

    public async Task HandleCallbackAsync(CallbackQuery callbackQuery, CancellationToken ct)
    {
        var chatId = callbackQuery.Message!.Chat.Id;

        var pokerSession = await cache.GetAsync<PokerSession>($"{BaseKey}{chatId}");
        if (pokerSession == null)
        {
            await bot.AnswerCallbackQuery(
                callbackQuery.Id,
                "Session not found.",
                cancellationToken: ct
            );
            return;
        }

        PokerGameLogic.NextStage(pokerSession);

        var stateJson = JsonSerializer.Serialize(pokerSession);
        await cache.SetAsync($"{BaseKey}{chatId}", pokerSession);
        await repository.UpdateGameAsync(chatId, GameName, stateJson);

        await bot.SendMessage(
            chatId,
            $"Next Stage: {pokerSession.Stage}. Table: {string.Join(", ", pokerSession.TableCards)}",
            cancellationToken: ct
        );

        await bot.AnswerCallbackQuery(callbackQuery.Id, "Action processed", cancellationToken: ct);
    }
}
