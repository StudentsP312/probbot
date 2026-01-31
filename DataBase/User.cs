using System.ComponentModel.DataAnnotations;

namespace Probbot.Database;

public class User
{
    [Key]
    public int Id {get; set;}

    [Required]
    public long TelegramId{get; set;}

    public int? Age {get; set;}

    public decimal Balanse {get; set;} = 0m;

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public DateTime LastUpdate {get; set;} = DateTime.UtcNow;

    public void UpdateBalance(decimal amount)
    {
        Balance += amount;
        LastUpdateAt = DateTime.UtcNow;
    }
}