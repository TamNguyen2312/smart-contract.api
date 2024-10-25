using Microsoft.AspNetCore.Identity;

namespace FS.BaseModels.IdentityModels;

public partial class RoleClaim : IdentityRoleClaim<long>
{
    public virtual Role Role { get; set; }
}

public partial class UserClaim : IdentityUserClaim<long>
{
    public virtual ApplicationUser User { get; set; }
}

public partial class UserLogin : IdentityUserLogin<long>
{
    public virtual ApplicationUser User { get; set; }
}

public partial class UserRole : IdentityUserRole<long>
{
    public virtual ApplicationUser User { get; set; }
    public virtual Role Role { get; set; }
}

public partial class UserToken : IdentityUserToken<long>
{
    public virtual  ApplicationUser User { get; set; }
}