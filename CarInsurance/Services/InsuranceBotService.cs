﻿using CarInsurance.Entities.Dto;
using CarInsurance.Entities;
using CarInsurance.Helper;
using CarInsurance.Persistence;
using CarInsurance.Repositories;
using MongoDB.Driver;
using Telegram.Bot;
using static MongoDB.Driver.Builders<CarInsurance.Entities.User>;
namespace CarInsurance.Services
{
    public class InsuranceBotService(IImageProcessingRepository imageProcessingService,
       IDbContext context, MissingDocumentsInspector empty,
       ITelegramBotClient client, AiChating aiChating) : IInsuranceBotService
    {
        private readonly AiChating _aiChating = aiChating;
        private readonly MissingDocumentsInspector _empty = empty;
        private readonly IImageProcessingRepository _imageProcessingService = imageProcessingService;
        private readonly IDbContext _context = context;

        public async Task HandleUserMessageAsync(UserMessageDto message, IBotClient botClient, CancellationToken ct)
        {
            var user = await _context
                .Users
                .Find(user => user.Id == message.UserId)
                .FirstOrDefaultAsync();
            if (message.Photos?.Any() == true )
            {
                if (!_empty.IsPassportEmpty(user.Passport) && !_empty.IsTechPassportEmpty(user.TechnicalPassports))
                {
                   await MessagesForPhoto(message, botClient, ct);
                }
                else if (_empty.IsPassportEmpty(user.Passport))
                {
                    await ProcessPhotoPassportAsync(message, botClient, ct);


                }
                else
                {
                    await ProcessPhotoTechPassportAsync(message, botClient, ct);
                }
            }
            else if (message.Text?.StartsWith("/start") == true)
            {
                await SendWelcomeMessage(message, botClient, ct);
            }
            else if (message.Text?.StartsWith("/changePassport") == true)
            {
                var filter = Filter.Eq(u => u.Id, message.UserId);
                var update = Builders<User>.Update
                    .Set(u => u.Passport, new Passport());
                await _context.Users.UpdateOneAsync(filter, update);
                await RequestDocument(message, botClient, ct);
            }
            else if (message.Text?.StartsWith("/changetechpassport") == true)
            {
                var filter = Filter.Eq(u => u.Id, message.UserId);
                var update = Builders<User>.Update
                    .Set(u => u.TechnicalPassports, new TechnicalPassport());
                await _context.Users.UpdateOneAsync(filter, update);
                await RequestDocument(message, botClient, ct);
            }
            else if (message.Text?.StartsWith("/buyinsurance") == true)
            {
                await SendInsurancePdfAsync(client, botClient, message);
            }
            else
            {
                await RequestDocument(message, botClient, ct);
            }
        }

        public async Task SendInsurancePdfAsync(ITelegramBotClient client, IBotClient botClient, UserMessageDto message)
        {
            var user = await _context
                .Users
                .Find(user => user.Id == message.UserId)
                .FirstOrDefaultAsync();
            await botClient.SendDocumentAsync(client, message, user);
        }
        private async Task ProcessPhotoPassportAsync(UserMessageDto message, IBotClient botClient, CancellationToken ct)
        {
            await botClient.SendMessageAsync(message.ChatId, "Дякую за фото! Обробляю дані...", ct);

            foreach (var photo in message.Photos)
            {
                var result = await _imageProcessingService.ProcessIdCardAsync(photo, ct, message.UserId);
                await botClient.SendMessageAsync(message.ChatId, $"Результати обробки:\n{result}", ct);
            }
        }

        private async Task ProcessPhotoTechPassportAsync(UserMessageDto message, IBotClient botClient, CancellationToken ct)
        {
            await botClient.SendMessageAsync(message.ChatId, "Дякую за фото! Обробляю дані...", ct);

            foreach (var photo in message.Photos)
            {
                var result = await _imageProcessingService.ProcessTechPassportAsync(photo, ct, message.UserId);
                await botClient.SendMessageAsync(message.ChatId, $"Результати обробки:\n{result}", ct);
                
            }
        }
        protected async Task SendWelcomeMessage(UserMessageDto message, IBotClient botClient, CancellationToken ct)
        {
            var user = await _context
                .Users
                .Find(user => user.Id == message.UserId)
                .FirstOrDefaultAsync();
            if (user is null)
            {
                var newUser = new User()
                {
                    Id = message.UserId
                };
                await _context.Users.InsertOneAsync(newUser);
            }

            await botClient.SendMessageAsync(
                message.ChatId,
                "Привіт! Я допоможу вам придбати автострахування. Надішліть фото паспорта або техпаспорта.",
                ct);

        }

        private async Task MessagesForPhoto(UserMessageDto message, IBotClient botClient, CancellationToken ct)
        {
            var messages = await _aiChating.MessagesForPhoto(message.Text, message.ChatId);
            await botClient.SendMessageAsync(
                message.ChatId,
                messages,
                ct);
        }
        private async Task RequestDocument(UserMessageDto message, IBotClient botClient, CancellationToken ct)
        {
            var user = await _context
                .Users
                .Find(user => user.Id == message.UserId)
                .FirstOrDefaultAsync();
            if (!_empty.IsPassportEmpty(user.Passport) && !_empty.IsTechPassportEmpty(user.TechnicalPassports))
            {
                var messages = await _aiChating.Messages(message.Text, message.ChatId,3);
                await botClient.SendMessageAsync(
                    message.ChatId,
                    messages,
                    ct);
            }
            else if (!_empty.IsTechPassportEmpty(user.TechnicalPassports))
            {
                var messages = await _aiChating.Messages(message.Text, message.ChatId, 2);
                await botClient.SendMessageAsync(
                    message.ChatId,
                    messages ,
                    ct);
            }
            else
            {
                var messages = await _aiChating.Messages(message.Text, message.ChatId, 1);
                await botClient.SendMessageAsync(
                     message.ChatId,
                     messages,
                     ct);
            }
        }
    }
}
