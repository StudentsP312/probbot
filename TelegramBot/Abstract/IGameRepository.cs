namespace TelegramBot.Abstract;

public interface IGameRepository
{
    Task CreateGameAsync(long userId, string gameName, string initialState);
    Task UpdateGameAsync(long userId, string gameName, string state);
    Task FinishGameAsync(long userId, string gameName, string finalState, decimal result);
    Task<string?> GetActiveGameStateAsync(long userId, string gameName);
}
