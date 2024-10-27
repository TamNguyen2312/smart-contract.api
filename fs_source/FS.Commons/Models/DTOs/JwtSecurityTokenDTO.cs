using System;
using System.IdentityModel.Tokens.Jwt;

namespace FS.Commons.Models.DTOs;

public class JwtSecurityTokenDTO
{
    public string AccessToken { get; set; } = null!;
    public JwtSecurityToken JwtToken { get; set; } = null!;
}
