using System;
using System.ComponentModel.DataAnnotations;

namespace FS.Commons.Models.DTOs;

public class ForgotPassDTO
{
    [Required(ErrorMessage = Constants.Required)]
    [Display(Name = "Email"), StringLength(255, ErrorMessage = Constants.MaxlengthError)]
    [EmailAddress(ErrorMessage = Constants.EmailAddressFormatError)]
    public string Email { get; set; } = null!;
}
