using System;
using FS.BaseModels.IdentityModels;
using FS.Common.Models.Models.Interfaces;

namespace FS.Commons.Models.DTOs;

public class UserViewDTO
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public string? Avatar { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public List<string>? roles { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender Gender { get; set; }

    public UserViewDTO(ApplicationUser user, List<string> roles)
    {
        Id = user.Id;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
        Username = user.UserName;
        FirstName = user.FirstName;
        LastName = user.LastName;
        FullName = $"{user.FirstName} {user.LastName}";
        Avatar = user.Avatar;
        Gender = Enum.TryParse<Gender>(user.Gender, out var gender) ? gender : default;
    }
}


