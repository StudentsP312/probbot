// Не забудьте свои namespace:
using Console.Advanced;
using Console.Advanced.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;

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

var host = builder.Build();
await host.RunAsync();
