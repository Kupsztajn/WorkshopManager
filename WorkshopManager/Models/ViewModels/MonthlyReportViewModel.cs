namespace WorkshopManager.Models.ViewModels;

public class MonthlyReportViewModel
{
    public int Month { get; set; }
    public int Year { get; set; }
    public List<MonthlyReportItem> ReportItems { get; set; }
    public decimal TotalCost;
}

public class MonthlyReportItem
{
    public ApplicationUser Client;
    public Vehicle Vehicle;
    public decimal TotalCost;
    public int OrderCount;
}