using System;
using System.ComponentModel.DataAnnotations;

namespace FS.Commons.Models.DTOs;

public class ChangePasswordDTO
{
    [Required(ErrorMessage = Constants.Required)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = Constants.Required)]
    [Display(Name = "Mật khẩu"), StringLength(255, ErrorMessage = Constants.PasswordStringLengthError, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [RegularExpression(Constants.REGEX_PASSWORD, ErrorMessage = Constants.PasswordInvalidFormat)]
    public string NewPassword { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = Constants.Required)]
    [Compare("NewPassword", ErrorMessage = Constants.ConfirmPasswordError)]
    [Display(Name = "Nhập lại mật khẩu"), StringLength(255, ErrorMessage = Constants.MaxlengthError)]
    public string ConfirmPassword { get; set; } = null!;
}
