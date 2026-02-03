using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Abstract;

namespace TelegramBot.Services.Games;

public class MinesGameService(ITelegramBotClient bot) : IGameService
{
    public string GameName => "Mines";

    public async Task StartGameAsync(long chatId, CancellationToken ct)
    {
        await bot.SendMessage(
            chatId,
            "Welcome to Mines Game! (Work in progress)",
            cancellationToken: ct
        );
    }

    public async Task HandleCallbackAsync(CallbackQuery callbackQuery, CancellationToken ct)
    {
        await bot.AnswerCallbackQuery(
            callbackQuery.Id,
            "Mines interaction...",
            cancellationToken: ct
        );
    }
}
