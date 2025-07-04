using Autofac;
using Autofac.Extensions.DependencyInjection;
using FastEndpoints;
using Serilog;
using UniCast.Application.TelegramBot;
using UniCast.Infrastructure.Caching;
using UniCast.Infrastructure.Moodle;
using UniCast.Infrastructure.Moodle.Configuration;
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
            containerBuilder.RegisterModule<TelegramBotApplicationModule>();

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
            containerBuilder.RegisterModule<MoodleInfrastructureModule>();
            containerBuilder.RegisterModule<CachingInfrastructureModule>();
        });

    builder.Host.UseSerilog((_, lc) => { lc.WriteTo.Console().WriteTo.Seq(builder.Configuration["Seq:Url"]!); });
    builder.Services.AddHttpClient();

    builder.Services.AddMemoryCache();

    builder.Services.ConfigureTelegramBot<Microsoft.AspNetCore.Http.Json.JsonOptions>(opt => opt.SerializerOptions);
    builder.Services.Configure<MoodleConfiguration>(builder.Configuration.GetSection("Moodle"));

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