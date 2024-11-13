using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FS.BaseModels.IdentityModels;
using FS.Common.Models.Models.Interfaces;
using FS.Commons;
using FS.Commons.Interfaces;
using FS.Commons.Models;

namespace App.Entity.DTOs.Profile;

public class ProfileUpdateDto : IEntity<ApplicationUser>
{
    [Required(ErrorMessage = Constants.Required)]
    public long Id { get; set; }
    
    /// <summary>
    /// email
    /// </summary>
    [Display(Name = "Email"), StringLength(255, ErrorMessage = Constants.MaxlengthError)]
    [EmailAddress(ErrorMessage = Constants.EmailAddressFormatError)]
    public string? Email { get; set; }
    
    /// <summary>
    /// PhoneNumber
    /// </summary>
    [StringLength(11, ErrorMessage = "Số điện thoại chỉ chứa tối đa 11 số!")]
    public string? PhoneNumber { get; set; }
    
    
    /// <summary>
    /// FirstName
    /// </summary>
    [Required(ErrorMessage = Constants.Required)]
    [Display(Name = "Họ"), StringLength(255, ErrorMessage = Constants.MaxlengthError)]
    public string? FirstName { get; set; }

    /// <summary>
    /// LastName
    /// </summary>
    [Required(ErrorMessage = Constants.Required)]
    [Display(Name = "Tên"), StringLength(255, ErrorMessage = Constants.MaxlengthError)]
    public string? LastName { get; set; }
    
    
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
    /// ngày tháng năm sinh
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
    
    public string CheckValidDateOfBirth()
    {
        if (DateOfBirth.HasValue)
        {
            var age = DateTime.Now.Year - DateOfBirth.Value.Year;
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

        return "VALID";
    }
    public string? IdentityCard { get; set; }
    
    public string CheckValidIdentityCard()
    {
        if (!string.IsNullOrEmpty(IdentityCard))
        {
            string pattern = @"^0(0[1-9]|[1-8][0-9]|9[0-6])[0-3]([0-9][0-9])[0-9]{6}$";
            if (!Regex.IsMatch(IdentityCard, pattern))
            {
                return "Giấy tờ tuỳ thân không đúng định dạng.";
            }
            return "VALID";            
        }

        return "VALID"; 
    }

    public ApplicationUser GetEntity()
    {
        return new ApplicationUser
        {
            FirstName = FirstName,
            LastName = LastName,
            Gender = Gender.Value.ToString(),
            IdentityCard = IdentityCard,
            Email = Email,
            PhoneNumber = PhoneNumber
        };
    }
    
    
}