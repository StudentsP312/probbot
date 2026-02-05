using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Abstract;
using TelegramBot.Services.Games;

namespace TelegramBot.Services;

public class UpdateHandler(
    ITelegramBotClient bot,
    ILogger<UpdateHandler> logger,
    MainMenuService mainMenuService,
    PokerGameService pokerGame,
    BlackjackGameService blackjackGame,
    MinesGameService minesGame
) : IUpdateHandler
{
    public async Task HandleErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        HandleErrorSource source,
        CancellationToken cancellationToken
    )
    {
        logger.LogError(exception, "HandleError: {Source}", source);
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (
            update switch
            {
                { Message: { } message } => OnMessage(message, cancellationToken),
                { CallbackQuery: { } callbackQuery } => OnCallbackQuery(
                    callbackQuery,
                    cancellationToken
                ),
                _ => Task.CompletedTask,
            }
        );
    }

    private async Task OnMessage(Message msg, CancellationToken ct)
    {
        if (msg.Text is not { } messageText)
            return;

        logger.LogInformation("Receive message: {Text} from {ChatId}", messageText, msg.Chat.Id);

        if (messageText.StartsWith("/start"))
        {
            await mainMenuService.SendMainMenuAsync(msg.Chat.Id, ct);
        }
        else
        {
            await bot.SendMessage(
                msg.Chat.Id,
                "Use /start to open the main menu.",
                cancellationToken: ct
            );
        }
    }

    private async Task OnCallbackQuery(CallbackQuery callbackQuery, CancellationToken ct)
    {
        if (callbackQuery.Data is not { } data)
            return;
        if (callbackQuery.Message is not { } message)
            return;

        logger.LogInformation("Received callback: {Data} from {ChatId}", data, message.Chat.Id);

        var task = data switch
        {
            "menu_profile" => mainMenuService.HandleProfileAsync(message.Chat.Id, ct),
            "game_poker" => pokerGame.StartGameAsync(message.Chat.Id, ct),
            "game_blackjack" => blackjackGame.StartGameAsync(message.Chat.Id, ct),
            "game_mines" => minesGame.StartGameAsync(message.Chat.Id, ct),
            _ when data.StartsWith("poker_") => pokerGame.HandleCallbackAsync(callbackQuery, ct),
            _ when data.StartsWith("bj_") => blackjackGame.HandleCallbackAsync(callbackQuery, ct),
            _ when data.StartsWith("mines_") => minesGame.HandleCallbackAsync(callbackQuery, ct),
            _ => bot.AnswerCallbackQuery(callbackQuery.Id, "Unknown action", cancellationToken: ct),
        };

        await task;

        // Always answer callback query to stop loading animation in client
        try
        {
            await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
        }
        catch
        { /* Ignore if already answered */
        }
    }
}
