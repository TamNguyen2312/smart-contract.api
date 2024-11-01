using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace FS.BaseModels.IdentityModels;

public partial class ApplicationUser : IdentityUser<long>
{
    public virtual ICollection<UserClaim> Claims { get; set; }
    public virtual ICollection<UserLogin> Logins { get; set; }
    public virtual ICollection<UserToken> Tokens { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Avatar { get; set; }
    public string? Gender { get; set; }
    [AllowNull]
    public DateTime? DateOfBirth { get; set; }
    [AllowNull]
    public string? IdentityCard { get; set; }
}