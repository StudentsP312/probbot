namespace TelegramBot.Models;

public enum Suit
{
    Hearts,
    Diamonds,
    Spades,
    Clubs,
}

public enum Rank
{
    Two = 2,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace,
}

public record Card(Suit Suit, Rank Rank)
{
    public override string ToString()
    {
        string s = Suit switch
        {
            Suit.Hearts => " ♥",
            Suit.Diamonds => "♦",
            Suit.Spades => "♠",
            Suit.Clubs => "♣",
        };
        string r = Rank switch
        {
            Rank.Ten => "10",
            Rank.Jack => "J",
            Rank.Queen => "Q",
            Rank.King => "K",
            Rank.Ace => "A",
            _ => ((int)Rank).ToString(),
        };
        return $"[{r}, {s}]";
    }
}

public class Deck
{
    private List<Card> _cards = new();

    public Deck()
    {
        foreach (Suit s in Enum.GetValues(typeof(Suit)))
        foreach (Rank r in Enum.GetValues(typeof(Rank)))
            _cards.Add(new Card(s, r));
        _cards = _cards.OrderBy(x => Guid.NewGuid()).ToList();
    }

    public Card Deal()
    {
        var c = _cards[0];
        _cards.RemoveAt(0);
        return c;
    }
}
