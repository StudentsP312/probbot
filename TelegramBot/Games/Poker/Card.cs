namespace TelegramBot.Games.Poker;

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
            _ => throw new ArgumentException("Card suit is not valid"),
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
    public List<Card> Cards { get; set; } = new();

    public Deck()
    {
        foreach (Suit s in Enum.GetValues(typeof(Suit)))
        foreach (Rank r in Enum.GetValues(typeof(Rank)))
            Cards.Add(new Card(s, r));
        Cards = Cards.OrderBy(x => Guid.NewGuid()).ToList();
    }

    public Card Deal()
    {
        if (Cards.Count == 0) throw new InvalidOperationException("Deck is empty");
        var c = Cards[0];
        Cards.RemoveAt(0);
        return c;
    }
}
