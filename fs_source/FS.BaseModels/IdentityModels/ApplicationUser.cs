using Microsoft.AspNetCore.Identity;

namespace FS.BaseModels.IdentityModels;

public partial class ApplicationUser : IdentityUser<long>
{
    public virtual ICollection<UserClaim> Claims { get; set; }
    public virtual ICollection<UserLogin> Logins { get; set; }
    public virtual ICollection<UserToken> Tokens { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Avatar { get; set; }
}