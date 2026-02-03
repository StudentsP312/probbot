using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Services;

public class MainMenuService(ITelegramBotClient bot)
{
    public async Task SendMainMenuAsync(long chatId, CancellationToken ct)
    {
        var keyboard = new InlineKeyboardMarkup(
            new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("👤 Profile", "menu_profile") },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🃏 Poker", "game_poker"),
                    InlineKeyboardButton.WithCallbackData("🎴 Blackjack", "game_blackjack"),
                },
                new[] { InlineKeyboardButton.WithCallbackData("💣 Mines", "game_mines") },
            }
        );

        await bot.SendMessage(
            chatId,
            "Hello, I'm ProbBot! Choose an option:",
            replyMarkup: keyboard,
            cancellationToken: ct
        );
    }

    public async Task HandleProfileAsync(long chatId, CancellationToken ct)
    {
        // Placeholder for profile info
        await bot.SendMessage(
            chatId,
            "👤 *Your Profile*\n\nBalance: 1000 🪙\nGames Played: 0",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: ct
        );
    }
}
