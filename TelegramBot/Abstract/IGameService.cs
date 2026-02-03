using Telegram.Bot.Types;

namespace TelegramBot.Abstract;

public interface IGameService
{
    string GameName { get; }
    Task StartGameAsync(long chatId, CancellationToken ct);
    Task HandleCallbackAsync(CallbackQuery callbackQuery, CancellationToken ct);
}
