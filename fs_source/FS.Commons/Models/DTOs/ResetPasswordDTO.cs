using System;
using System.ComponentModel.DataAnnotations;

namespace FS.Commons.Models.DTOs;

public class ResetPasswordDTO
{
    [Required(ErrorMessage = Constants.Required)]
    public long UserId { get; set; }

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

    /// <summary>
    /// token xác thực đúng người dùng để thay đổi mật khẩu
    /// </summary>
    public string Token { get; set; } = null!;
}
