using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WorkshopManager.Models.ViewModels;

namespace WorkshopManager.Pdf;

public class MonthlyReportDocument : IDocument
{
    private readonly MonthlyReportViewModel _model;

    public MonthlyReportDocument(MonthlyReportViewModel model)
    {
        _model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);
            page.Header().Text($"Raport dla {
                CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(_model.Month)
            } {_model.Year}").FontSize(20).Bold();

            page.Content().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.ConstantColumn(100);
                    columns.ConstantColumn(100);
                });

                table.Header(header =>
                {
                    header.Cell().Text("Klient");
                    header.Cell().Text("Pojazd");
                    header.Cell().Text("Koszt");
                    header.Cell().Text("Ilość zleceń");
                });

                foreach (var item in _model.ReportItems)
                {
                    table.Cell().Text($"{item.Client.Name} {item.Client.Surname}");
                    table.Cell().Text(item.Vehicle.Registration);
                    table.Cell().Text(item.TotalCost.ToString("C"));
                    table.Cell().Text(item.OrderCount.ToString());
                }

                table.Footer(footer =>
                {
                    footer.Cell().ColumnSpan(3).AlignRight().Text("Łącznie:");
                    footer.Cell().Text(_model.TotalCost.ToString("C"));
                });
            });
        });
    }
}