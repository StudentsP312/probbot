using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramBot;
using TelegramBot.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<BotConfiguration>(builder.Configuration.GetSection("BotConfiguration"));

builder
    .Services.AddHttpClient("telegram_bot_client")
    .AddTypedClient<ITelegramBotClient>(
        (httpClient, sp) =>
        {
            BotConfiguration? botConfig = sp.GetService<IOptions<BotConfiguration>>()?.Value;
            ArgumentNullException.ThrowIfNull(botConfig);

            TelegramBotClientOptions options = new(botConfig.BotToken);
            return new TelegramBotClient(options, httpClient);
        }
    );

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<ReceiverService>();
builder.Services.AddHostedService<PollingService>();

// Register new services
builder.Services.AddScoped<MainMenuService>();
builder.Services.AddScoped<TelegramBot.Services.Games.PokerGameService>();
builder.Services.AddScoped<TelegramBot.Services.Games.BlackjackGameService>();
builder.Services.AddScoped<TelegramBot.Services.Games.MinesGameService>();

var host = builder.Build();
await host.RunAsync();
