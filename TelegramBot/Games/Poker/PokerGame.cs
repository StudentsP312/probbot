using TelegramBot.Games.Poker;

namespace TelegramBot.Games.Poker;

public static class PokerGameLogic
{
    public static void StartGame(PokerSession session)
    {
        // Reset or Initialize if needed, though session usually comes fresh
        if (session.Deck.Cards.Count < 52) 
            session.Deck = new Deck(); // Ensure fresh deck

        session.PlayerHand.Clear();
        session.EnemyHand.Clear();
        session.TableCards.Clear();
        session.Stage = 0;

        session.PlayerHand.Add(session.Deck.Deal());
        session.PlayerHand.Add(session.Deck.Deal());

        session.EnemyHand.Add(session.Deck.Deal());
        session.EnemyHand.Add(session.Deck.Deal());
    }

    public static void NextStage(PokerSession session)
    {
        if (session.Stage == 0)
        {
            for (int i = 0; i < 3; i++)
                session.TableCards.Add(session.Deck.Deal());
        }
        else if (session.Stage == 1 || session.Stage == 2)
        {
            session.TableCards.Add(session.Deck.Deal());
        }
        session.Stage++;
    }

    public static string GetWinner(PokerSession session)
    {
        var playerRes = PokerHandResult.FromHand(session.PlayerHand, session.TableCards);
        var enemyRes = PokerHandResult.FromHand(session.EnemyHand, session.TableCards);

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