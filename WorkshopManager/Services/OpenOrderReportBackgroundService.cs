using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WorkshopManager.Controllers;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Pdf;

namespace WorkshopManager.Services;

public class OpenOrderReportBackgroundService : BackgroundService
{
    private readonly ILogger<OpenOrderReportBackgroundService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IServiceProvider _serviceProvider;

    public OpenOrderReportBackgroundService(ILogger<OpenOrderReportBackgroundService> logger,
        IServiceProvider serviceProvider
        )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("OpenOrderReportBackgroundService is starting.");
        while (cancellationToken.IsCancellationRequested == false)
        {
            try
            {
                await SendEmailAsync();
                _logger.LogInformation($"Email sent at {DateTime.Now}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error sending email");
            }
            
            await Task.Delay(TimeSpan.FromHours(24), cancellationToken);
        }
    }

    private Stream GeneratePdfReport()
    {
        UsersDbContext _context = _serviceProvider.GetRequiredService<UsersDbContext>();
        
        List<ServiceOrder> orders = _context.ServiceOrders
            .Include(o => o.Vehicle)
            .Include(o => o.Vehicle.Client)
            .Include(o => o.CreatedAt)
            .Where(o => o.CompletedAt != null).ToList();

        OpenOrderReportDocument doc = new OpenOrderReportDocument(orders);
        QuestPDF.Settings.License = LicenseType.Community;
        byte[] pdfBytes = doc.GeneratePdf();
        Stream contentStream = new MemoryStream(pdfBytes);
        return contentStream;
    }
    
    private async Task SendEmailAsync()
    {
        return;
        
        // Yeah no
        using var smtp = new SmtpClient("smtp.example.com")
        {
            Port = 587,
            Credentials = new System.Net.NetworkCredential("email@email.com", "haslo"),
        };

        var mail = new MailMessage
        {
            From = new MailAddress("email@email.com"),
            Subject = "Dzienny Raport",
            Body = $"W załączniku: raport z dnia {DateTime.Now.Date:dd/MM/yyyy}.",
            IsBodyHtml = false
        };
        foreach (string email in _userManager.GetUsersInRoleAsync("Admin").Result.Select(u => u.Email))
        {
            mail.To.Add(new MailAddress(email));
        }
        mail.Attachments.Add(new Attachment(GeneratePdfReport(), $"report{DateTime.Now:M-d-yy}.pdf"));

        await smtp.SendMailAsync(mail);
    }
}