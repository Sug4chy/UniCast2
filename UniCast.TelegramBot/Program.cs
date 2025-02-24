using Autofac;
using Autofac.Extensions.DependencyInjection;
using FastEndpoints;
using UniCast.Application.TelegramBot;
using UniCast.Infrastructure.Telegram;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterModule<TelegramBotApplicationModule>();

        containerBuilder.RegisterModule(new TelegramInfrastructureModule
        {
            BotToken = builder.Configuration["TelegramBot:Token"] ?? string.Empty,
            WebhookUrl = builder.Configuration["WEBHOOK_URL"] ?? string.Empty,
        });
    });

builder.Services.AddFastEndpoints();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddAsyncInitialization();

var app = builder.Build();

app.UseRouting();
app.UseFastEndpoints();

await app.InitAndRunAsync();