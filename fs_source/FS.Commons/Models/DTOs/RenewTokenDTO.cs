using System;
using System.ComponentModel.DataAnnotations;

namespace FS.Commons.Models.DTOs;

public class RenewTokenDTO
{
    [Required(ErrorMessage = Constants.Required)]
    public string AccessToken { get; set; } = null!;
    [Required(ErrorMessage = Constants.Required)]
    public string RefreshToken { get; set; } = null!;
}
