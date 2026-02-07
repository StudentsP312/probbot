//using TelegramBot.Games.Mine.Casino;
using System;
using System.Collections.Generic;

namespace Mine.Net
{
    public enum CellType
    {
        Empty,
        SmallWin,
        MediumWin,
        BigWin,
        Jackpot,
        Bomb,
        Multiplier,
        Mystery
    }

    public enum CellState
    {
        Hidden,
        Revealed,
        Flagged,
        Locked
    }

    public enum GameState
    {
        Playing,
        Won,
        Lost,
        Cashout
    }

    public enum RiskLevel
    {
        Low,
        Medium,
        High,
        Insane
    }

    public class CasinoCell
    {
        public CellType Type { get; set; }
        public CellState State { get; set; }
        public decimal WinAmount { get; set; }
        public double Multiplier { get; set; }
        public int ClusterSize { get; set; }

        public CasinoCell()
        {
            Type = CellType.Empty;
            State = CellState.Hidden;
            WinAmount = 0;
            Multiplier = 1.0;
            ClusterSize = 0;
        }
    }

    public class CasinoConfig
    {
        public int BoardSize { get; set; }
        public int BombCount { get; set; }
        public int JackpotCount { get; set; }
        public int MultiplierCount { get; set; }
        public RiskLevel Risk { get; set; }
        public decimal BaseBet { get; set; }
        public decimal CurrentBalance { get; set; }
        public Dictionary<CellType, int> Distribution { get; set; }

        public CasinoConfig(RiskLevel risk = RiskLevel.Medium, decimal baseBet = 10.0m)
        {
            Risk = risk;
            BaseBet = baseBet;
            CurrentBalance = 1000.0m;

            switch (risk)
            {
                case RiskLevel.Low:
                    BoardSize = 6;
                    BombCount = 3;
                    JackpotCount = 1;
                    MultiplierCount = 2;
                    Distribution = new Dictionary<CellType, int>
                    {
                        { CellType.Empty, 40 }, { CellType.SmallWin, 30 }, { CellType.MediumWin, 15 },
                        { CellType.BigWin, 8 }, { CellType.Jackpot, 2 }, { CellType.Bomb, 3 },
                        { CellType.Multiplier, 1 }, { CellType.Mystery, 1 }
                    };
                    break;

                case RiskLevel.Medium:
                    BoardSize = 8;
                    BombCount = 8;
                    JackpotCount = 2;
                    MultiplierCount = 3;
                    Distribution = new Dictionary<CellType, int>
                    {
                        { CellType.Empty, 35 }, { CellType.SmallWin, 25 }, { CellType.MediumWin, 15 },
                        { CellType.BigWin, 10 }, { CellType.Jackpot, 3 }, { CellType.Bomb, 8 },
                        { CellType.Multiplier, 2 }, { CellType.Mystery, 2 }
                    };
                    break;

                case RiskLevel.High:
                    BoardSize = 10;
                    BombCount = 15;
                    JackpotCount = 3;
                    MultiplierCount = 4;
                    Distribution = new Dictionary<CellType, int>
                    {
                        { CellType.Empty, 30 }, { CellType.SmallWin, 20 }, { CellType.MediumWin, 15 },
                        { CellType.BigWin, 12 }, { CellType.Jackpot, 4 }, { CellType.Bomb, 15 },
                        { CellType.Multiplier, 3 }, { CellType.Mystery, 1 }
                    };
                    break;

                case RiskLevel.Insane:
                    BoardSize = 12;
                    BombCount = 25;
                    JackpotCount = 5;
                    MultiplierCount = 6;
                    Distribution = new Dictionary<CellType, int>
                    {
                        { CellType.Empty, 25 }, { CellType.SmallWin, 15 }, { CellType.MediumWin, 15 },


{ CellType.BigWin, 15 }, { CellType.Jackpot, 5 }, { CellType.Bomb, 20 },
                        { CellType.Multiplier, 4 }, { CellType.Mystery, 1 }
            }
            ;
            break;
        }
        }

        public CasinoConfig(int size, int bombs, int jackpots, decimal bet)
        {
            BoardSize = size;
            BombCount = bombs;
            JackpotCount = jackpots;
            BaseBet = bet;
            Risk = RiskLevel.Medium;
            CurrentBalance = 1000.0m;
            MultiplierCount = Math.Max(1, size / 4);

            Distribution = new Dictionary<CellType, int>
            {
                { CellType.Empty, 40 }, { CellType.SmallWin, 20 }, { CellType.MediumWin, 15 },
                { CellType.BigWin, 10 }, { CellType.Jackpot, 2 }, { CellType.Bomb, 10 },
                { CellType.Multiplier, 2 }, { CellType.Mystery, 1 }
            };
        }
    }

    public abstract class CasinoEvent
{
    public (int X, int Y)? Position { get; }
    public bool IsGameOver { get; }
    public decimal AmountChange { get; }
    public CellType Type { get; }

    protected CasinoEvent((int X, int Y)? pos = null, bool over = false,
                         decimal amount = 0, CellType type = CellType.Empty)
    {
        Position = pos;
        IsGameOver = over;
        AmountChange = amount;
        Type = type;
    }
}

public class CellOpenedEvent : CasinoEvent
{
    public decimal Win { get; }
    public double Multiplier { get; }
    public int Cluster { get; }

    public CellOpenedEvent(int x, int y, CellType type, decimal win,
                          double mult = 1.0, int cluster = 0, bool over = false)
        : base((x, y), over, win, type)
    {
        Win = win;
        Multiplier = mult;
        Cluster = cluster;
    }
}

public class BombEvent : CasinoEvent
{
    public decimal Loss { get; }
    public int BombsLeft { get; }

    public BombEvent(int x, int y, decimal loss, int left)
        : base((x, y), true, -loss, CellType.Bomb)
    {
        Loss = loss;
        BombsLeft = left;
    }
}

public class JackpotEvent : CasinoEvent
{
    public decimal Jackpot { get; }
    public bool IsMega { get; }

    public JackpotEvent(int x, int y, decimal amount, bool mega = false)
        : base((x, y), false, amount, CellType.Jackpot)
    {
        Jackpot = amount;
        IsMega = mega;
    }
}

public class MultiplierEvent : CasinoEvent
{
    public double NewMultiplier { get; }
    public double CellMultiplier { get; }

    public MultiplierEvent(int x, int y, double cellMult, double newMult)
        : base((x, y), false, 0, CellType.Multiplier)
    {
        CellMultiplier = cellMult;
        NewMultiplier = newMult;
    }
}

public class MysteryEvent : CasinoEvent
{
    public string Effect { get; }
    public decimal Bonus { get; }

    public MysteryEvent(int x, int y, string effect, decimal bonus)
        : base((x, y), false, bonus, CellType.Mystery)
    {
        Effect = effect;
        Bonus = bonus;
    }
}

public class EmptyClusterEvent : CasinoEvent
{
    public int Size { get; }
    public decimal Bonus { get; }

    public EmptyClusterEvent(int x, int y, int size, decimal bonus)
        : base((x, y), false, bonus, CellType.Empty)
    {
        Size = size;
        Bonus = bonus;
    }
}

public class CashoutEvent : CasinoEvent
{
    public decimal Total { get; }
    public decimal Profit { get; }

    public CashoutEvent(decimal total, decimal profit)
        : base(null, true, total, CellType.Empty)
    {
        Total = total;
        Profit = profit;
    }
}


public interface IRandom
{
    int Next(int max);
    int Next(int min, int max);
    double NextDouble();
}

public class SystemRandom : IRandom
{
    private readonly Random rand = new Random();
    public int Next(int max) => rand.Next(max);
    public int Next(int min, int max) => rand.Next(min, max);
    public double NextDouble() => rand.NextDouble();
}

public interface IFieldGenerator
{
    void Generate(CasinoCell[,] board, CasinoConfig config, (int X, int Y) safe);
    decimal CellValue(CellType type, decimal bet, double mult);
}

public class CasinoGenerator : IFieldGenerator
{
    private readonly IRandom random;

    public CasinoGenerator(IRandom rand)
    {
        random = rand;
    }

    public void Generate(CasinoCell[,] board, CasinoConfig config, (int X, int Y) safe)
    {
        int size = board.GetLength(0);
        var positions = new List<(int X, int Y)>();

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (Math.Abs(x - safe.X) > 1 || Math.Abs(y - safe.Y) > 1)
                {
                    positions.Add((x, y));
                    board[x, y] = new CasinoCell();
                }
                else
                {
                    board[x, y] = new CasinoCell { Type = CellType.Empty };
                }
            }
        }

        Shuffle(positions);
        PlaceSpecial(board, positions, config);
        FillRest(board, positions, config);
        CalculateWins(board, config);
        FindClusters(board);
    }

    private void PlaceSpecial(CasinoCell[,] board, List<(int X, int Y)> pos, CasinoConfig config)
    {
        int idx = 0;

        for (int i = 0; i < config.BombCount && idx < pos.Count; i++, idx++)
        {
            var p = pos[idx];
            board[p.X, p.Y].Type = CellType.Bomb;
        }

        for (int i = 0; i < config.JackpotCount && idx < pos.Count; i++, idx++)
        {
            var p = pos[idx];
            board[p.X, p.Y].Type = CellType.Jackpot;
        }

        for (int i = 0; i < config.MultiplierCount && idx < pos.Count; i++, idx++)
        {
            var p = pos[idx];
            board[p.X, p.Y].Type = CellType.Multiplier;
            board[p.X, p.Y].Multiplier = RandomMultiplier();
        }

        int mystery = Math.Max(1, (config.BoardSize * config.BoardSize) / 100);
        for (int i = 0; i < mystery && idx < pos.Count; i++, idx++)
        {
            var p = pos[idx];
            board[p.X, p.Y].Type = CellType.Mystery;
        }
    }

    private void FillRest(CasinoCell[,] board, List<(int X, int Y)> pos, CasinoConfig config)
    {
        int idx = config.BombCount + config.JackpotCount + config.MultiplierCount;
        var types = new List<CellType>();

        foreach (var kv in config.Distribution)
        {
            for (int i = 0; i < kv.Value; i++) types.Add(kv.Key);
        }

        Shuffle(types);

        int typeIdx = 0;
        while (idx < pos.Count && typeIdx < types.Count)
        {
            var p = pos[idx];
            var type = types[typeIdx % types.Count];

            if (type == CellType.Bomb || type == CellType.Jackpot ||
                    type == CellType.Multiplier || type == CellType.Mystery)
                {
                typeIdx++;
                continue;
            }

            board[p.X, p.Y].Type = type;
            idx++;
            typeIdx++;
        }

        while (idx < pos.Count)
        {
            var p = pos[idx];
            board[p.X, p.Y].Type = CellType.Empty;
            idx++;
        }
    }


private void CalculateWins(CasinoCell[,] board, CasinoConfig config)
    {
        int size = board.GetLength(0);
        double mult = 1.0;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (board[x, y].Type == CellType.Multiplier)
                    mult *= board[x, y].Multiplier;
            }
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                board[x, y].WinAmount = CellValue(board[x, y].Type, config.BaseBet, mult);
            }
        }
    }

    public decimal CellValue(CellType type, decimal bet, double mult)
    {
        return type switch
        {
            CellType.Empty => 0,
            CellType.SmallWin => bet * 0.5m * (decimal)mult,
            CellType.MediumWin => bet * 2.0m * (decimal)mult,
            CellType.BigWin => bet * 5.0m * (decimal)mult,
            CellType.Jackpot => bet * 100.0m * (decimal)mult,
            CellType.Bomb => -bet * 10.0m,
            CellType.Multiplier => 0,
            CellType.Mystery => bet * (decimal)(random.NextDouble() * 10.0) * (decimal)mult,
            _ => 0
        };
    }

    private void FindClusters(CasinoCell[,] board)
    {
        int size = board.GetLength(0);
        bool[,] visited = new bool[size, size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (!visited[x, y] && board[x, y].Type == CellType.Empty)
                {
                    int cluster = Flood(board, visited, x, y);
                    Mark(board, visited, x, y, cluster);
                }
            }
        }
    }

    private int Flood(CasinoCell[,] board, bool[,] visited, int x, int y)
    {
        int size = board.GetLength(0);
        if (x < 0 ||x >= size ||y < 0 || y>=size ||
                visited[x, y] || board[x, y].Type != CellType.Empty)
                return 0;

        visited[x, y] = true;
        int count = 1;

        count += Flood(board, visited, x + 1, y);
        count += Flood(board, visited, x - 1, y);
        count += Flood(board, visited, x, y + 1);
        count += Flood(board, visited, x, y - 1);

        return count;
    }

    private void Mark(CasinoCell[,] board, bool[,] visited, int startX, int startY, int cluster)
    {
        int size = board.GetLength(0);
        var stack = new Stack<(int X, int Y)>();
        stack.Push((startX, startY));

        while (stack.Count > 0)
        {
            var (x, y) = stack.Pop();
            board[x, y].ClusterSize = cluster;

            if (x + 1 < size && visited[x + 1, y]) stack.Push((x + 1, y));
            if (x - 1 >= 0 && visited[x - 1, y]) stack.Push((x - 1, y));
            if (y + 1 < size && visited[x, y + 1]) stack.Push((x, y + 1));
            if (y - 1 >= 0 && visited[x, y - 1]) stack.Push((x, y - 1));

            visited[x, y] = false;
        }
    }

    private double RandomMultiplier()
    {
        double[] mults = { 1.5, 2.0, 2.5, 3.0, 5.0 };
        return mults[random.Next(mults.Length)];
    }

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}

public interface IWinSystem
{
    decimal TotalWin(decimal bet, int cells, double mult);
    decimal ClusterBonus(int size, decimal bet);
    decimal BombPenalty(decimal bet, int bombs);
    (decimal Amount, string Effect) MysteryEffect(decimal bet);
}


public class CasinoWinSystem : IWinSystem
{
    private readonly IRandom random;

    public CasinoWinSystem(IRandom rand)
    {
        random = rand;
    }

    public decimal TotalWin(decimal bet, int cells, double mult)
    {
        decimal baseWin = bet * cells * 0.1m;
        decimal multBonus = baseWin * (decimal)(mult - 1.0);
        decimal randomBonus = baseWin * (decimal)((random.NextDouble() - 0.5) * 0.4);
        return Math.Max(0, baseWin + multBonus + randomBonus);
    }

    public decimal ClusterBonus(int size, decimal bet)
    {
        if (size < 3) return 0;
        return bet * (decimal)Math.Pow(2, size - 2);
    }

    public decimal BombPenalty(decimal bet, int bombs)
    {
        return bet * 10.0m * (decimal)Math.Pow(1.5, bombs - 1);
    }

    public (decimal Amount, string Effect) MysteryEffect(decimal bet)
    {
        int type = random.Next(6);

        return type switch
        {
            0 => (bet * 5.0m, "Bonus"),
            1 => (bet * 10.0m, "Surprise"),
            2 => (bet * 2.0m, "Luck"),
            3 => (-bet * 3.0m, "Penalty"),
            4 => (0, "Nothing"),
            5 => (bet * 20.0m, "Mega"),
            _ => (0, "Unknown")
        };
    }
}

public class CasinoMineGame
{
    private CasinoCell[,] board;
    private CasinoConfig config;
    private GameState gameState;
    private int flags;
    private int revealed;
    private int bombs;
    private bool first;
    private double multiplier;
    private decimal winnings;

    private readonly IFieldGenerator generator;
    private readonly IWinSystem winSystem;
    private readonly IRandom random;
    private readonly Queue<CasinoEvent> events;

    private int streak;
    private int jackpots;
    private int mysteries;
    private List<int> clusters;

    public int BoardSize => config.BoardSize;
    public int TotalBombs => config.BombCount;
    public int Flags => flags;
    public int Revealed => revealed;
    public GameState State => gameState;
    public CasinoCell[,] Board => board;
    public Queue<CasinoEvent> Events => events;
    public decimal Balance => config.CurrentBalance;
    public decimal Winnings => winnings;
    public double Multiplier => multiplier;
    public int BombsLeft => config.BombCount - bombs;

    public CasinoMineGame(CasinoConfig cfg, IFieldGenerator gen = null, IWinSystem wins = null)
    {
        config = cfg;
        generator = gen ?? new CasinoGenerator(new SystemRandom());
        winSystem = wins ?? new CasinoWinSystem(new SystemRandom());
        random = new SystemRandom();
        events = new Queue<CasinoEvent>();

        Reset();
    }

    private void Reset()
    {
        board = new CasinoCell[config.BoardSize, config.BoardSize];
        first = true;
        flags = 0;
        revealed = 0;
        bombs = 0;
        gameState = GameState.Playing;
        multiplier = 1.0;
        winnings = 0;
        streak = 0;
        jackpots = 0;
        mysteries = 0;
        clusters = new List<int>();

        config.CurrentBalance -= config.BaseBet;
        events.Clear();
    }

    public bool OpenCell(int x, int y)
    {
        if (gameState != GameState.Playing)
            return false;

        if (!Valid(x, y))
            return false;

        var cell = board[x, y];

        if (cell.State == CellState.Revealed || cell.State == CellState.Locked)
            return false;

        if (first)
        {
            generator.Generate(board, config, (x, y));
            first = false;
        }


switch (cell.Type)
        {
            case CellType.Bomb:
                return HandleBomb(x, y, cell);
            case CellType.Empty:
                return HandleEmpty(x, y, cell);
            case CellType.Multiplier:
                return HandleMultiplier(x, y, cell);
            case CellType.Mystery:
                return HandleMystery(x, y, cell);
            default:
                return HandleWin(x, y, cell);
        }
    }

    private bool HandleBomb(int x, int y, CasinoCell cell)
    {
        cell.State = CellState.Revealed;
        bombs++;

        decimal penalty = winSystem.BombPenalty(config.BaseBet, bombs);
        config.CurrentBalance += penalty;
        winnings += penalty;

        events.Enqueue(new BombEvent(x, y, -penalty, BombsLeft));
        streak = 0;

        if (bombs >= Math.Max(1, config.BombCount / 3))
        {
            gameState = GameState.Lost;
            ShowAllBombs();
            return false;
        }

        return true;
    }

    private bool HandleEmpty(int x, int y, CasinoCell cell)
    {
        var cluster = OpenCluster(x, y);
        int size = cluster.Count;

        if (size >= 3)
        {
            decimal bonus = winSystem.ClusterBonus(size, config.BaseBet);
            config.CurrentBalance += bonus;
            winnings += bonus;
            clusters.Add(size);
            events.Enqueue(new EmptyClusterEvent(x, y, size, bonus));
        }

        revealed += size;
        streak++;
        CheckWin();
        return true;
    }

    private List<CasinoCell> OpenCluster(int startX, int startY)
    {
        var cluster = new List<CasinoCell>();
        var stack = new Stack<(int X, int Y)>();
        stack.Push((startX, startY));

        while (stack.Count > 0)
        {
            var (x, y) = stack.Pop();

            if (!Valid(x, y) || board[x, y].State == CellState.Revealed)
                continue;

            if (board[x, y].Type == CellType.Empty ||
               (Math.Abs(x - startX) <= 1 && Math.Abs(y - startY) <= 1))
            {
                board[x, y].State = CellState.Revealed;
                cluster.Add(board[x, y]);

                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        stack.Push((x + dx, y + dy));
                    }
                }
            }
        }

        return cluster;
    }

    private bool HandleMultiplier(int x, int y, CasinoCell cell)
    {
        cell.State = CellState.Revealed;
        revealed++;

        double old = multiplier;
        multiplier *= cell.Multiplier;
        events.Enqueue(new MultiplierEvent(x, y, cell.Multiplier, multiplier));
        streak++;
        CheckWin();
        return true;
    }

    private bool HandleMystery(int x, int y, CasinoCell cell)
    {
        cell.State = CellState.Revealed;
        revealed++;

        var (bonus, effect) = winSystem.MysteryEffect(config.BaseBet);
        config.CurrentBalance += bonus;
        winnings += bonus;
        mysteries++;

        events.Enqueue(new MysteryEvent(x, y, effect, bonus));
        streak++;
        CheckWin();
        return true;
    }


private bool HandleWin(int x, int y, CasinoCell cell)
    {
        cell.State = CellState.Revealed;
        revealed++;

        decimal win = cell.WinAmount * (decimal)multiplier;
        config.CurrentBalance += win;
        winnings += win;

        bool mega = false;
        if (cell.Type == CellType.Jackpot)
        {
            jackpots++;
            streak += 5;

            if (jackpots >= 2 && streak >= 10)
            {
                mega = true;
                win *= 2.0m;
            }

            events.Enqueue(new JackpotEvent(x, y, win, mega));
        }
        else
        {
            events.Enqueue(new CellOpenedEvent(x, y, cell.Type, win, multiplier));
            streak++;
        }

        CheckWin();
        return true;
    }

    public bool ToggleFlag(int x, int y)
    {
        if (gameState != GameState.Playing)
            return false;

        if (!Valid(x, y))
            return false;

        var cell = board[x, y];

        if (cell.State == CellState.Revealed || cell.State == CellState.Locked)
            return false;

        if (cell.State == CellState.Flagged)
        {
            cell.State = CellState.Hidden;
            flags--;
        }
        else
        {
            cell.State = CellState.Flagged;
            flags++;
        }

        return true;
    }

    public bool Cashout()
    {
        if (gameState != GameState.Playing)
            return false;

        gameState = GameState.Cashout;

        decimal total = winnings + winSystem.TotalWin(config.BaseBet, revealed, multiplier);
        decimal profit = total - config.BaseBet;
        config.CurrentBalance += total;

        events.Enqueue(new CashoutEvent(total, profit));
        return true;
    }

    private void CheckWin()
    {
        int safe = (config.BoardSize * config.BoardSize) - config.BombCount;
        if (revealed >= safe * 0.8)
        {
            gameState = GameState.Won;
            Cashout();
        }
    }

    private void ShowAllBombs()
    {
        for (int y = 0; y < config.BoardSize; y++)
        {
            for (int x = 0; x < config.BoardSize; x++)
            {
                if (board[x, y].Type == CellType.Bomb && board[x, y].State != CellState.Revealed)
                {
                    board[x, y].State = CellState.Revealed;
                }
            }
        }
    }

    private bool Valid(int x, int y)
    {
        return x >= 0 && x < config.BoardSize && y >= 0 && y < config.BoardSize;
    }

    public void NewGame(CasinoConfig newConfig = null)
    {
        if (newConfig != null) config = newConfig;
        Reset();
    }

    public Dictionary<string, object> Stats()
    {
        return new Dictionary<string, object>
            {
                { "Winnings", winnings },
                { "Revealed", revealed },
                { "Bombs", bombs },
                { "Jackpots", jackpots },
                { "Mysteries", mysteries },
                { "Multiplier", multiplier },
                { "Streak", streak },
                { "AvgCluster", clusters.Count > 0 ? clusters.Average() : 0 },
                { "MaxCluster", clusters.Count > 0 ? clusters.Max() : 0 }
            };
    }
}
}
