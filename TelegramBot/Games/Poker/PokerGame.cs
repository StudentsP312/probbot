using TelegramBot.Models;

namespace TelegramBot.Games.Poker;

public class TexasHoldemGame
{
    public Deck Deck { get; private set; }

    public List<Card> PlayerHand { get; private set; }

    public List<Card> EnemyHand { get; private set; }

    public List<Card> Table { get; private set; }

    public int Stage { get; private set; }

    public TexasHoldemGame()
    {
        Deck = new Deck();
        PlayerHand = new List<Card>();
        EnemyHand = new List<Card>();
        Table = new List<Card>();
        Stage = 0;
    }

    public void StartGame()
    {
        PlayerHand.Add(Deck.Deal());
        PlayerHand.Add(Deck.Deal());

        EnemyHand.Add(Deck.Deal());
        EnemyHand.Add(Deck.Deal());
    }

    public void NextStage()
    {
        if (Stage == 0)
        {
            for (int i = 0; i < 3; i++)
                Table.Add(Deck.Deal());
        }
        else if (Stage == 1 || Stage == 2)
            Table.Add(Deck.Deal());
        Stage++;
    }

    public string GetWinner()
    {
        var playerRes = PokerHandResult.Evaluate(PlayerHand, Table);
        var enemyRes = PokerHandResult.Evaluate(EnemyHand, Table);

        if (playerRes.Strength > enemyRes.Strength)
            return "Player win";
        if (playerRes.Strength < enemyRes.Strength)
            return "Enemy win";
        if (playerRes.HighRank > enemyRes.HighRank)
            return "Player win";
        if (playerRes.HighRank < enemyRes.HighRank)
            return "Enemy win";

        return "Draw";
    }
}
