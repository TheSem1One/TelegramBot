namespace TelegramBot.Domain.Options
{
    public class ConnectionOptions
    {
        public const string SectionName = "ConnectionStrings";
        public string DbConnection { get; set; } = null!;
        public string DbName { get; set; } = null!;
        public string DbCollection { get; set; } = null!;
        public string MindeeAPI { get; set; } = null!;
        public string TelegramAPI { get; set; } = null!;
    }
}
