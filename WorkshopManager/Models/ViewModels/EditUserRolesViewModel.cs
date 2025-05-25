using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.Models.ViewModels;

public class EditUserRolesViewModel
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string SelectedRole { get; set; } // jedna wybrana rola
    public List<string> AllRoles { get; set; } // lista dostępnych ról
}

public class RoleSelection
{
    public string RoleName { get; set; }
    public bool Selected { get; set; }
}