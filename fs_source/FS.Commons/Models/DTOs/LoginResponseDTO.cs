using System;

namespace FS.Commons.Models.DTOs;

public class LoginResponseDTO
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string? Redirect { get; set; }
}
