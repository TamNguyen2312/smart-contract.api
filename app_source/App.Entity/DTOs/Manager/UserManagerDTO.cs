using FS.BaseModels.IdentityModels;

namespace App.Entity.DTOs.Manager;

public class UserManagerDTO
{
    public ApplicationUser User { get; set; }
    public Entities.Manager Manager { get; set; }
}