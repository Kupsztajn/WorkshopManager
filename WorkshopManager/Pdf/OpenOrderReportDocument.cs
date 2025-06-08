using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WorkshopManager.Models;
using WorkshopManager.Models.ViewModels;

namespace WorkshopManager.Pdf;

public class OpenOrderReportDocument : IDocument
{
    private readonly List<ServiceOrder> _orders;

    public OpenOrderReportDocument(List<ServiceOrder> orders)
    {
        _orders = orders;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);
            page.Header().Text($"Aktywne zlecenia w dniu {DateTime.Now:dd/MM/yyyy}").FontSize(20).Bold();

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
                    header.Cell().Text("Data otwarcia");
                    header.Cell().Text("Dotychczasowy koszt");
                });

                foreach (var order in _orders)
                {
                    table.Cell().Text($"{order.Vehicle.Client.Name} {order.Vehicle.Client.Surname}");
                    table.Cell().Text(order.Vehicle.Registration);
                    table.Cell().Text(order.CreatedAt.ToString("dd/MM/yyyy"));
                    table.Cell().Text(order.ServiceTasks.Sum(st => st.LaborCost + st.UsedParts.Sum(up => up.TotalCost)).ToString("C"));
                }

                table.Footer(footer =>
                {
                    footer.Cell().ColumnSpan(3).AlignRight().Text("Łącznie zleceń:");
                    footer.Cell().Text(_orders.Count.ToString());
                });
            });
        });
    }
}