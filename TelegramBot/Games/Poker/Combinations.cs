using TelegramBot.Games.Poker;

public enum HandStrength
{
    HighCard,
    Pair,
    TwoPair,
    ThreeOfAkind,
    Straight,
    Flush,
    FullHouse,
    FourOfAkind,
    StraightFlush,
}

public class PokerHandResult
{
    public HandStrength Strength { get; set; }
    public Rank HighRank { get; set; }

    public static PokerHandResult Evaluate(List<Card> hand, List<Card> table)
    {
        var allCards = hand.Concat(table).OrderByDescending(c => c.Rank).ToList();
        var groups = allCards
            .GroupBy(c => c.Rank)
            .OrderByDescending(g => g.Count())
            .ThenByDescending(g => g.Key)
            .ToList();
        if (groups.Any(g => g.Count() == 3) && groups.Any(g => g.Count() == 2))
            return new PokerHandResult
            {
                Strength = HandStrength.FullHouse,
                HighRank = groups.First(g => g.Count() == 3).Key,
            };
        if (groups.Any(g => g.Count() == 3))
            return new PokerHandResult
            {
                Strength = HandStrength.ThreeOfAkind,
                HighRank = groups[0].Key,
            };
        if (groups.Any(g => g.Count() == 2))
            return new PokerHandResult { Strength = HandStrength.Pair, HighRank = groups[0].Key };
        return new PokerHandResult
        {
            Strength = HandStrength.HighCard,
            HighRank = allCards[0].Rank,
        };
    }
}
