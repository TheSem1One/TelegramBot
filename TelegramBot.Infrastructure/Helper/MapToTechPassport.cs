using Mindee.Parsing.Common;
using Mindee.Product.Generated;
using TelegramBot.Domain.Entity;

namespace TelegramBot.Infrastructure.Helper
{
    public class MapToTechPassport
    {
        public TechnicalPassport ToConvertTechPassportDto(Document<GeneratedV1> document)
        {
            var prediction = document.Inference.Prediction;
            var techPassport = new TechnicalPassport
            {
                Mark = MapToString(prediction.Fields["mark"].ToString()),
                MaximumMass = MapToString(prediction.Fields["mass"].ToString()),
                Capacity = MapToString(prediction.Fields["capacity"].ToString()),
                Color = MapToString(prediction.Fields["color"].ToString()),
                CommercialDescription = MapToString(prediction.Fields["commertial_description"].ToString()),
                Type = MapToString(prediction.Fields["type"].ToString()),
                VehicleIdentificationNumber = MapToString(prediction.Fields["vehicle_identification_number"].ToString())
            };


            return techPassport;
        }

        private string MapToString(string value)
        {
            var cleaner = value.Substring(11).Trim();
            return cleaner;
        }
    }
}
