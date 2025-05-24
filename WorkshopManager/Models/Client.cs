namespace WorkshopManager.Models;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    // inne pola według potrzeb
    // Relacja do recepcjonisty (użytkownika Identity), który dodał klienta
    public string AddedByUserId { get; set; }
    public ApplicationUser AddedByUser { get; set; }
}
