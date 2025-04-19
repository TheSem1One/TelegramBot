using MongoDB.Bson.Serialization.Attributes;

namespace TelegramBot.Domain.Entity.Dto
{
    public class TechPassportDto
    {
        public string? Mark { get; set; }
        public string? Type { get; set; }
        public string? CommercialDescription { get; set; }
        public string? VehicleIdentificationNumber { get; set; }
        public string? MaximumMass { get; set; }
        public string? Capacity { get; set; }
        public string? Color { get; set; }
    }
}
