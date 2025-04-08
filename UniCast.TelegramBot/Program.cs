using Autofac;
using Autofac.Extensions.DependencyInjection;
using FastEndpoints;
using Serilog;
using UniCast.Application.TelegramBot;
using UniCast.Infrastructure.Moodle;
using UniCast.Infrastructure.Persistence;
using UniCast.Infrastructure.Telegram;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .ConfigureContainer<ContainerBuilder>(containerBuilder =>
        {
            containerBuilder.RegisterModule(new TelegramBotApplicationModule
            {
                BotUsername = builder.Configuration["TelegramBot:Username"] ?? string.Empty
            });

            containerBuilder.RegisterModule(new TelegramInfrastructureModule
            {
                BotToken = builder.Configuration["TelegramBot:Token"] ?? string.Empty,
                WebhookUrl = builder.Configuration["WEBHOOK_URL"] ?? string.Empty,
                SetWebhook = true,
                CertificatePath = builder.Configuration["WEBHOOK_CERTIFICATE_PATH"] ?? string.Empty
            });
            containerBuilder.RegisterModule(new PersistenceInfrastructureModule
            {
                ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty
            });
            containerBuilder.RegisterModule(new MoodleInfrastructureModule
            {
                MoodleBaseUrl = builder.Configuration["Moodle:BaseUrl"] ?? string.Empty
            });
        });

    builder.Host.UseSerilog((_, lc) => { lc.WriteTo.Console(); });
    builder.Services.AddHttpClient();

    builder.Services.ConfigureTelegramBot<Microsoft.AspNetCore.Http.Json.JsonOptions>(opt => opt.SerializerOptions);

    builder.Services.AddFastEndpoints();
    builder.Services.AddRouting(options => options.LowercaseUrls = true);

    builder.Services.AddAsyncInitialization();

    var app = builder.Build();
    app.UseSerilogRequestLogging();

    app.UseRouting();
    app.UseFastEndpoints();

    await app.InitAndRunAsync();
}
catch (Exception e)
{
    Log.Error(e, "Unhandled exception while initializing Telegram Bot.");
}
finally
{
    Log.CloseAndFlush();
}