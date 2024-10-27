using System;
using System.ComponentModel.DataAnnotations;

namespace FS.Commons.Models.DTOs;

public class LoginDTO
{
    /// <summary>
    /// The email.
    /// </summary>
    public string Email => Username;
    [Required(ErrorMessage = Constants.Required)]
    [EmailAddress(ErrorMessage = Constants.EmailAddressFormatError)]
    [Display(Name = "Email"), StringLength(255, ErrorMessage = Constants.MaxlengthError)]
    public string Username { get; set; } = null!;
    [Required(ErrorMessage = Constants.Required)]
    public string Password { get; set; } = null!;
    public bool IsRemember { get; set; }
    public long RUUID { get; set; }
    public string? Redirect { get; set; }
}
