using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TelegramBot.Domain.Entity
{
    public class User
    {
        [BsonElement("_id"), BsonRepresentation(BsonType.String)]
        public long? Id { get; set; }


        [BsonElement("Passport")]
        public Passport Passport { get; set; } = new Passport();

        [BsonElement("TechnicalPassports")]
        public TechnicalPassport TechnicalPassports { get; set; } = new TechnicalPassport();
    }

    public class Passport
    {
        [BsonElement("Name")]
        public string? Name { get; set; }

        [BsonElement("Surname")]
        public string? Surname { get; set; }

        [BsonElement("DocumentNumber")]
        public string? DocumentNumber { get; set; }

        [BsonElement("Sex")]
        public string? Sex { get; set; }

        [BsonElement("DateOfBirth")]
        public DateOnly? DateOfBirth { get; set; }

    }

    public class TechnicalPassport
    {
        [BsonElement("Mark")]
        public string? Mark { get; set; }

        [BsonElement("Type")]
        public string? Type { get; set; }

        [BsonElement("CommercialDescription")]
        public string? CommercialDescription { get; set; }

        [BsonElement("VehicleIdentificationNumber")]
        public string? VehicleIdentificationNumber { get; set; }

        [BsonElement("MaximumMass")]
        public string? MaximumMass { get; set; }

        [BsonElement("Capacity")]
        public string? Capacity { get; set; }

        [BsonElement("Color")]
        public string? Color { get; set; }

    }
}
