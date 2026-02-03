using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Abstract;

namespace TelegramBot.Services.Games;

public class PokerGameService(ITelegramBotClient bot) : IGameService
{
    public string GameName => "Poker";

    public async Task StartGameAsync(long chatId, CancellationToken ct)
    {
        await bot.SendMessage(
            chatId,
            "Welcome to Poker! (Work in progress)",
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
