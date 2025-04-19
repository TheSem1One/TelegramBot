using CarInsurance.Entities;

namespace CarInsurance.Helper
{
    public class MissingDocumentsInspector
    {
        public bool IsPassportEmpty(Passport passport)
        {
            if (passport == null)
                return true;

            return string.IsNullOrWhiteSpace(passport.Name)
                   && string.IsNullOrWhiteSpace(passport.Surname)
                   && string.IsNullOrWhiteSpace(passport.Sex)
                   && passport.DateOfBirth == null;
        }

        public bool IsTechPassportEmpty(TechnicalPassport passport)
        {
            if (passport == null)
                return true;

            return string.IsNullOrWhiteSpace(passport.CommercialDescription)
                   && string.IsNullOrWhiteSpace(passport.Capacity)
                   && string.IsNullOrWhiteSpace(passport.Color)
                   && string.IsNullOrWhiteSpace(passport.Mark)
                   && string.IsNullOrWhiteSpace(passport.MaximumMass)
                   && string.IsNullOrWhiteSpace(passport.Type)
                   && string.IsNullOrWhiteSpace(passport.VehicleIdentificationNumber);

        }
    }
}
