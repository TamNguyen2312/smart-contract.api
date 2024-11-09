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
    public string? PhoneNumber { get; set; }
    public string Gender { get; set; }
    public string Status { get; set; }
    public string IdentityCard { get; set; }
    public List<string>? Roles { get; set; }

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
        Gender = user.Gender ?? "None";
        Status = user.Status;
        IdentityCard = user.IdentityCard;
        Roles = roles;
    }
}


