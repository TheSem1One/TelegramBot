using Mindee.Input;
namespace CarInsurance.Entities.Dto
{
    public class UserMessageDto
    {
        public long ChatId { get; set; }
        public long UserId { get; set; }
        public string Text { get; set; }
        public IEnumerable<LocalInputSource> Photos { get; set; }
    }
}
