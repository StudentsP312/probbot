using Microsoft.Extensions.Logging;
using TelegramBot.Abstract;

namespace TelegramBot.Services;

public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
    : PollingServiceBase<ReceiverService>(serviceProvider, logger);
