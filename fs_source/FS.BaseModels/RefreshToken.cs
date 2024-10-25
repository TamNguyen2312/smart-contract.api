using System;
using FS.BaseModels.IdentityModels;

namespace FS.BaseModels;

public partial class RefreshToken
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string JwtId { get; set; }
    public string Token { get; set; }
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiredAt { get; set; }
    //
    public virtual ApplicationUser User { get; set; }
}
