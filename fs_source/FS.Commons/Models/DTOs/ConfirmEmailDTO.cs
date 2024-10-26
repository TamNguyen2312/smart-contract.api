using System;

namespace FS.Commons.Models.DTOs;

public class ConfirmEmailDTO
{
    public string UserId { get; set; } = null!;
    public string Token { get; set; } = null!;
}
