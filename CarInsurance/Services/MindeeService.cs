using CarInsurance.Entities;
using CarInsurance.Helper;
using CarInsurance.Options;
using CarInsurance.Persistence;
using CarInsurance.Repositories;
using Microsoft.Extensions.Options;
using Mindee.Http;
using Mindee.Input;
using Mindee.Product.Generated;
using Mindee.Product.InternationalId;
using Mindee;
using MongoDB.Driver;

namespace CarInsurance.Services
{
    public class MindeeService(IDbContext context, IOptions<ConnectionOptions> options,
     MapToTechPassport map) : IImageProcessingRepository
    {
        private readonly MapToTechPassport _map = map;
        private readonly IDbContext _context = context;
        private readonly MindeeClient _mindeeClient = new (options.Value.MindeeAPI);

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
                    DocumentNumber = prediction.DocumentNumber.Value,
                    Sex = prediction.Sex.Value
                };
                var update = Builders<User>.Update
                    .Set(u => u.Passport, passport);

                await _context.Users.UpdateOneAsync(userFilter, update);

                return $"✅ Успішно зчитано!\n" +
                       $"👤 Ім’я: {passport.Name}\n" +
                       $"👤 Прізвище: {passport.Surname}\n" +
                       $"🆔 Номер документа: {passport.DocumentNumber}\n" +
                       $"📅 Дата народження: {passport.DateOfBirth:dd.MM.yyyy}\n" +
                       $"🚻 Стать: {passport.Sex}\n+";
            }

            return "Не вдалось зі сканувати фото, будь ласка спробуйте ще раз";


        }

        public async Task<string> ProcessTechPassportAsync(LocalInputSource image, CancellationToken ct, long userId)
        {
            var userFilter = Builders<User>.Filter.Eq(u => u.Id, userId);

            var endpoint = new CustomEndpoint(
                endpointName: "techpassport",
                accountName: "JustSem1One",
                version: "1"
            );
            var response = await _mindeeClient.EnqueueAndParseAsync<GeneratedV1>(image, endpoint);

            var prediction = response.Document;
            if (prediction is not null)
            {
                var passport = _map.ToConvertTechPassportDto(prediction);
                var update = Builders<User>.Update
                    .Set(u => u.TechnicalPassports, passport);
                await _context.Users.UpdateOneAsync(userFilter, update);
                return $"✅ Успішно зчитано!\n" +
                       $"🚗 Марка: {passport.Mark}\n" +
                       $"🚘 Тип: {passport.Type}\n" +
                       $"📄 Комерційний опис: {passport.CommercialDescription}\n" +
                       $"🔢 VIN: {passport.VehicleIdentificationNumber}\n" +
                       $"⚖️ Максимальна маса: {passport.MaximumMass}\n" +
                       $"🛢️ Обʼєм двигуна: {passport.Capacity}\n" +
                       $"🎨 Колір: {passport.Color}";

            }

            return "Не вдалось зі сканувати фото, будь ласка спробуйте ще раз";
        }


    }
}
