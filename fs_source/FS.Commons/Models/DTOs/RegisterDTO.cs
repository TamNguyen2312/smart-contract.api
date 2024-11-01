using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FS.Common.Models.Models.Interfaces;

namespace FS.Commons.Models.DTOs;

public class RegisterDTO : ITrimmable, IValidatableObject
{
	/// <summary>
	/// FirstName
	/// </summary>
	[Required(ErrorMessage = Constants.Required)]
	[Display(Name = "Họ"), StringLength(255, ErrorMessage = Constants.MaxlengthError)]
	public string FirstName { get; set; }

	/// <summary>
	/// LastName
	/// </summary>
	[Required(ErrorMessage = Constants.Required)]
	[Display(Name = "Tên"), StringLength(255, ErrorMessage = Constants.MaxlengthError)]
	public string LastName { get; set; }

	/// <summary>
	/// Giới tính user
	/// </summary>
	[Display(Name = "Giới tính")]
	public Gender? Gender { get; set; }
	public bool IsValidGender()
	{
		if (Gender.HasValue)
			return Enum.IsDefined(typeof(Gender), Gender);
		return true;
	}

	/// <summary>
	/// PhoneNumber
	/// </summary>
	[Required(ErrorMessage = Constants.Required)]
	[StringLength(11, ErrorMessage = "Số điện thoại chỉ chứa tối đa 11 số!")]
	public string PhoneNumber { get; set; }

	/// <summary>
	/// email
	/// </summary>
	[Required(ErrorMessage = Constants.Required)]
	[Display(Name = "Email"), StringLength(255, ErrorMessage = Constants.MaxlengthError)]
	[EmailAddress(ErrorMessage = Constants.EmailAddressFormatError)]
	public string Email { get; set; }

	/// <summary>
	/// mật khẩu
	/// </summary>
	[Required(ErrorMessage = Constants.Required)]
	[Display(Name = "Mật khẩu"), StringLength(255, ErrorMessage = Constants.PasswordStringLengthError, MinimumLength = 6)]
	[DataType(DataType.Password)]
	[RegularExpression(Constants.REGEX_PASSWORD, ErrorMessage = Constants.PasswordInvalidFormat)]
	public string Password { get; set; }

	/// <summary>
	/// nhập lại mật khẩu
	/// </summary>
	[DataType(DataType.Password)]
	[Required(ErrorMessage = Constants.Required)]
	[Compare("Password", ErrorMessage = Constants.ConfirmPasswordError)]
	[Display(Name = "Nhập lại mật khẩu"), StringLength(255, ErrorMessage = Constants.MaxlengthError)]
	public string ConfirmPassword { get; set; }


	/// <summary>
	/// loại user
	/// </summary>
	[Required(ErrorMessage = Constants.Required)]
	public UserType UserType { get; set; }
	public bool IsValidUserType()
	{
		return Enum.IsDefined(typeof(UserType), UserType);
	}


	public void Trim()
	{
		FirstName = FirstName.Trim();
		LastName = LastName.Trim();
	}

	/// <summary>
	/// Ngày tháng năm sinh
	/// </summary>
	[Required(ErrorMessage = Constants.Required)]
	[DataType(DataType.Date)]
	public DateTime DateOfBirth { get; set; }
	public string CheckValidDateOfBirth()
	{
		var age = DateTime.Now.Year - DateOfBirth.Year;
		if (age <= 18)
		{
			return "Người dùng phải trên 18 tuổi.";
		}
		else if (age >= 65)
		{
			return "Người phải dưới 65 tuổi.";
		}
		return "VALID";
	}

	[Required(ErrorMessage = Constants.Required)]
	public string IdentityCard { get; set; } = null!;
	public string CheckValidIdentityCard()
	{
		string pattern = @"^0(0[1-9]|[1-8][0-9]|9[0-6])[0-3]([0-9][0-9])[0-9]{6}$";
		if (!Regex.IsMatch(IdentityCard, pattern))
		{
			return "Giấy tờ tuỳ thân không đúng định dạng.";
		}
		return "VALID";
	}

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		Trim();
		return new List<ValidationResult>();
	}
}
