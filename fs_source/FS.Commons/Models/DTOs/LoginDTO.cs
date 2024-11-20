using System;
using System.ComponentModel.DataAnnotations;

namespace FS.Commons.Models.DTOs;

public class LoginDTO
{
    [Required(ErrorMessage = Constants.Required)]
    [Display(Name = "Username or Email"), StringLength(255, ErrorMessage = Constants.MaxlengthError)]
    public string UserNameOrEmail { get; set; } = null!;

    [Required(ErrorMessage = Constants.Required)]
    public string Password { get; set; } = null!;

    public bool IsRemember { get; set; }
    public long RUUID { get; set; }
    public string? Redirect { get; set; }
}