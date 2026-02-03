using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Abstract;

namespace TelegramBot.Services.Games;

public class BlackjackGameService(ITelegramBotClient bot) : IGameService
{
    public string GameName => "Blackjack";

    public async Task StartGameAsync(long chatId, CancellationToken ct)
    {
        await bot.SendMessage(
            chatId,
            "Welcome to Blackjack! (Work in progress)",
            cancellationToken: ct
        );
    }

    public async Task HandleCallbackAsync(CallbackQuery callbackQuery, CancellationToken ct)
    {
        await bot.AnswerCallbackQuery(
            callbackQuery.Id,
            "Blackjack interaction...",
            cancellationToken: ct
        );
    }
}
