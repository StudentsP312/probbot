using TelegramBot.Games.Poker;

public class PokerSession
{
    public Deck Deck { get; set; } = new();
    public List<Card> PlayerHand { get; set; } = new();
    public List<Card> TableCards { get; set; } = new();
    public decimal CurrentPot { get; set; }
    public int Round { get; set; } = 0;
}
