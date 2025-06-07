namespace WorkshopManager.Models.ViewModels;

public class ReportViewModel
{
    public ApplicationUser Client { get; set; }
    public List<ReportItem> ReportItems { get; set; }
    public decimal TotalCost { get; set; }

    public int? SelectedMonth { get; set; }
    public int? SelectedVehicleId { get; set; }
    public List<Vehicle> Vehicles { get; set; }
}

public class ReportItem
{
    public DateTime Date { get; set; }
    public Vehicle Vehicle { get; set; }
    public decimal LaborCost { get; set; }
    public decimal PartsCost { get; set; }
}