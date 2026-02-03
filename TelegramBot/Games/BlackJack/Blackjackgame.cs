using TelegramBot.Games.CardModels;

namespace BlackJackGames;

public class BlackjackGame : IGame
{
    public string Name => "Блэкджек ♠️";
    private List<Card> _playerHand = new();
    private List<Card> _dealerHand = new();
    private Deck _deck = new();
    private bool _isOver = false;

    public async Task StartAsync(ITelegramBotClient bot, long chatId, User user, IDatabaseService db)
    {
        if (user.Balance < 50) { await bot.SendTextMessageAsync(chatId, "Минимальная ставка 50$."); return; }
        
        user.Balance -= 50;
        await db.SaveUserAsync(user);

        _playerHand = new() { _deck.Deal(), _deck.Deal() };
        _dealerHand = new() { _deck.Deal(), _deck.Deal() };

        await SendStatus(bot, chatId, "Игра началась!");
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, User user, IDatabaseService db)
    {
        if (_isOver || update.CallbackQuery == null) return;
        string action = update.CallbackQuery.Data;

        if (action == "bj_hit")
        {
            _playerHand.Add(_deck.Deal());
            if (CalculateScore(_playerHand) > 21) await Finish(bot, user, db, "Перебор! Вы проиграли.");
            else await SendStatus(bot, user.TelegramId, "Вы взяли карту.");
        }
        else if (action == "bj_stand")
        {
            while (CalculateScore(_dealerHand) < 17) _dealerHand.Add(_deck.Deal());
            int p = CalculateScore(_playerHand);
            int d = CalculateScore(_dealerHand);

            if (d > 21 || p > d) { user.Balance += 100; await db.SaveUserAsync(user); await Finish(bot, user, db, "Вы победили!"); }
            else if (p < d) await Finish(bot, user, db, "Дилер победил.");
            else { user.Balance += 50; await db.SaveUserAsync(user); await Finish(bot, user, db, "Ничья."); }
        }
    }

    private int CalculateScore(List<Card> hand)
    {
        int total = hand.Sum(c => (int)c.Rank > 10 ? 10 : (int)c.Rank);
        int aces = hand.Count(c => c.Rank == Rank.Ace);
        for(int i=0; i<aces; i++) if (total + 10 <= 21) total += 10;
        return total;
    }

    private async Task SendStatus(ITelegramBotClient bot, long chatId, string msg)
    {
        var kb = new InlineKeyboardMarkup(new[] {
            new[] { InlineKeyboardButton.WithCallbackData("Еще", "bj_hit"), InlineKeyboardButton.WithCallbackData("Хватит", "bj_stand") }
        });
        await bot.SendTextMessageAsync(chatId, $"{msg}\nВаши карты: {string.Join(" ", _playerHand)} ({CalculateScore(_playerHand)})\nДилер: {_dealerHand[0]} [?]", replyMarkup: kb);
    }

    private async Task Finish(ITelegramBotClient bot, User user, IDatabaseService db, string res)
    {
        _isOver = true;
        await bot.SendTextMessageAsync(user.TelegramId, $"{res}\nКарты дилера: {string.Join(" ", _dealerHand)} ({CalculateScore(_dealerHand)})\nВаш баланс: {user.Balance}$");
    }
}
