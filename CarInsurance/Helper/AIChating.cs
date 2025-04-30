using System.ClientModel;
using CarInsurance.Options;
using CarInsurance.Repositories;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

public class AiChating(IOptions<ConnectionOptions> option, IUserSessionService sessionService)
{
    private readonly IOptions<ConnectionOptions> _option = option;
    private readonly IUserSessionService _sessionService = sessionService;

    public async Task<string> Messages(string message, long chatId)
    {
        var session = _sessionService.GetOrCreate(chatId,message);
        var dbMessage = session.Result;

        var endpoint = new Uri("https://models.github.ai/inference");
        var model = "openai/gpt-4.1";
        var apiKey = _option.Value.OpenAIAPI;
        var credential = new ApiKeyCredential(apiKey);
        var openAIOptions = new OpenAIClientOptions() { Endpoint = endpoint };
        var client = new ChatClient(model, credential, openAIOptions);

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(dbMessage[0]),
        };
        for (int i = 1; i < dbMessage.Count; i++)
        {
            messages.Add(new UserChatMessage(dbMessage[i]));
        }
        var response = await client.CompleteChatAsync(messages);
        var reply = response.Value.Content[0].Text;
        dbMessage.Add(reply);

        await _sessionService.Save(chatId, reply); 

        return reply;
    }
}