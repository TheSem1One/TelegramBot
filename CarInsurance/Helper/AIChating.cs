using CarInsurance.Options;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.ClientModel.Primitives;


namespace CarInsurance.Helper
{
    public class AiChating(IOptions<ConnectionOptions> option)
    {
        private readonly IOptions<ConnectionOptions> _option=option;
        public async Task<string> Messages(string message)
        {
            var endpoint = new Uri("https://models.github.ai/inference");
            var model = "openai/gpt-4.1";
            var apiKey = _option.Value.OpenAIAPI;
            var credential = new ApiKeyCredential(apiKey);
            var openAIOptions = new OpenAIClientOptions()
            {
                Endpoint = endpoint
            };
            var client = new ChatClient(model, credential, openAIOptions);

            List<ChatMessage> messages = new List<ChatMessage>()
            {
                new SystemChatMessage("system", "Ти є страхувальний бот, " +
                                                "допоможи користувачу застрахувати авто та в кінці пропонуй купити страховку за 100$."),
                new UserChatMessage("user",message),
            };

            var response = client.CompleteChat(messages);
           return response.Value.Content[0].Text;
        }
    }
}
