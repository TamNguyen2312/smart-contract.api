using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FS.BaseModels.IdentityModels;
using FS.Common.Models.Models.Interfaces;
using FS.Commons.Interfaces;
using FS.Commons.Models;

namespace App.Entity.DTOs.Profile;

public class PersonalProfileDto : IEntity<ApplicationUser>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Avatar { get; set; }
    public Gender? Gender { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
    public string? IdentityCard { get; set; }
    
    public bool IsValidGender()
    {
        if (Gender.HasValue)
            return Enum.IsDefined(typeof(Gender), Gender);
        return true;
    }

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
            Avatar = Avatar,
            Gender = Gender.Value.ToString(),
            IdentityCard = IdentityCard
        };
    }
    
}