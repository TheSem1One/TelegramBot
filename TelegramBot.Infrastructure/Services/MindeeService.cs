using Microsoft.Extensions.Options;
using Mindee;
using Mindee.Http;
using Mindee.Input;
using Mindee.Product.InternationalId;
using MongoDB.Driver;
using TelegramBot.Domain.Entity;
using TelegramBot.Domain.Options;
using TelegramBot.Domain.Repositories;
using TelegramBot.Infrastructure.Persistence;
using Mindee.Product.Generated;
using TelegramBot.Infrastructure.Helper;

public class MindeeService(DbContext context, IOptions<ConnectionOptions> options,
    MapToTechPassport map) : IImageProcessingRepository
{
    private readonly MapToTechPassport _map = map;
    private readonly DbContext _context = context;
    private readonly MindeeClient _mindeeClient = new MindeeClient(options.Value.MindeeAPI);

    public async Task<string> ProcessIdCardAsync(LocalInputSource image, CancellationToken ct, long userId)
    {

        var response = await _mindeeClient.EnqueueAndParseAsync<InternationalIdV2>(image);

        var prediction = response.Document.Inference.Prediction;
        if (prediction is not null)
        {
            var userFilter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var passport = new Passport()
            {
                DateOfBirth = DateOnly.Parse(prediction.BirthDate.Value),
                Name = prediction.GivenNames.FirstOrDefault().Value,
                Surname = prediction.Surnames.FirstOrDefault().Value,
                Sex = prediction.Sex.Value
            };
            var update = Builders<User>.Update
                .Set(u => u.Passport, passport);

            await _context.Users.UpdateOneAsync(userFilter, update);

            return $"""
                        Ім'я: {prediction.GivenNames.FirstOrDefault()?.Value ?? "Не визначено"}
                        Прізвище: {prediction.Surnames.FirstOrDefault()}
                        Дата народження: {prediction.BirthDate.Value ?? "Не визначено"}
                        Номер документа: {prediction.DocumentNumber.Value ?? "Не визначено"}
                        """;
        }

        return "Не вдалось зі сканувати фото, будь ласка спробуйте ще раз";


    }

    public async Task<string> ProcessTechPassportAsync(LocalInputSource image, CancellationToken ct, long userId)
    {
        var userFilter = Builders<User>.Filter.Eq(u => u.Id, userId);

        var endpoint = new CustomEndpoint(
            endpointName: "test",
            accountName: "TheSem1One",
            version: "1"
        );
        var response = await _mindeeClient.EnqueueAndParseAsync<GeneratedV1>(image, endpoint);

        var prediction = response.Document;
        if (prediction is not null)
        {
            var passport = _map.ToConvertTechPassportDto(prediction);
            // Маппінг з урахуванням можливих ключів
            var update = Builders<User>.Update
                .Set(u => u.TechnicalPassports, passport);
            await _context.Users.UpdateOneAsync(userFilter, update);
            return "Дякую операція успішно виконана";
        }

        return "Не вдалось зі сканувати фото, будь ласка спробуйте ще раз";
    }


}