using Microsoft.AspNetCore.Identity;

namespace FS.BaseModels.IdentityModels;

public partial class Role : IdentityRole<long>
{
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<RoleClaim> RoleClaims { get; set; }
    public bool IsAdmin { get; set; }
}