using FS.BaseModels.IdentityModels;

namespace App.Entity.DTOs.Employee;

public class UserEmpDTO
{
    public ApplicationUser User { get; set; }
    public Entities.Employee Employee { get; set; }
}