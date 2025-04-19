using CarInsurance.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class InsuranceDocument(User user) : IDocument
{
    private readonly User _user = user;
    private const int InsuranceCost = 4200;
    private const int CoverageAmount = 600;
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        Random rand = new();
        var numberOfPolice = rand.Next(11111111, 999999999);
        container.Page(page =>
        {
            page.Margin(40);
            page.Size(PageSizes.A4);

            page.Header().Text("ТОВ Живи на здоров'я")
                .FontSize(20).Bold().AlignCenter().ParagraphSpacing(10);

            page.Content().Column(column =>
            {
                column.Spacing(10);

                column.Item().Text("СТРАХОВИЙ ПОЛІС").FontSize(18).Bold().Underline().AlignCenter();

                column.Item().Text($"Номер поліса: {numberOfPolice}");
                column.Item().Text($"Ім’я страхувальника:{_user.Passport.Surname} {_user.Passport.Name} ");
                column.Item().Text($"Паспорт: {_user.Passport.DocumentNumber}");

                column.Item().Text($"Марка авто: {_user.TechnicalPassports.Mark} {_user.TechnicalPassports.Type}");
                column.Item().Text($"VIN: {_user.TechnicalPassports.VehicleIdentificationNumber}");

                column.Item().Text($"Період дії: {DateTime.Today} – {DateTime.Today.AddYears(3)}");
                column.Item().Text($"Сума покриття: {CoverageAmount} грн");
                column.Item().Text($"Вартість поліса: {InsuranceCost} грн");

                column.Item().PaddingTop(20).Text("Підпис страхувальника: ________________________");

                column.Item().PaddingTop(40).Text("Цей поліс підтверджує укладення договору страхування відповідно до чинного законодавства України.")
                    .FontSize(10).Italic();
            });

            page.Footer().AlignCenter().Text($"Дата створення поліса: {DateTime.Now:dd.MM.yyyy}");
        });
    }
}