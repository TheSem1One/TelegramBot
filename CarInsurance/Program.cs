using CarInsurance.Data;
using CarInsurance.Helper;
using CarInsurance.Options;
using CarInsurance.Persistence;
using CarInsurance.Repositories;
using CarInsurance.Services;
using Microsoft.Extensions.Options;
using QuestPDF.Infrastructure;
using Telegram.Bot;


class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // ??????? appsettings.{Environment}.json
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.Services.Configure<ConnectionOptions>(
            builder.Configuration.GetSection(ConnectionOptions.SectionName));

        builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection(nameof(MongoDbSettings)));
        builder.Services.AddSingleton<DbContext, UserContext>();
        builder.Services.AddSingleton<IImageProcessingRepository, MindeeService>();
        builder.Services.AddSingleton<IInsuranceBotService, InsuranceBotService>();
        builder.Services.AddHostedService<TelegramBotClientAdapter>();
        builder.Services.AddSingleton<MapToTechPassport>();
        builder.Services.AddSingleton<IsEmpty>();
        builder.Services.AddSingleton<ITelegramBotClient>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<ConnectionOptions>>().Value;
            return new TelegramBotClient(options.TelegramAPI);
        });
        QuestPDF.Settings.License = LicenseType.Community;


        var app = builder.Build();

        await app.RunAsync();
    }
}