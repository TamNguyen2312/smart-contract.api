using System;

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
    public string? RoleName { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender Gender { get; set; }
}

public enum Gender
{
    None = 0,
    Male = 1,
    Female = 2,
    Other = 3
}
