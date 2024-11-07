using System.Diagnostics.CodeAnalysis;
using FS.Commons.Models;

namespace App.Entity.Entities;
public class Department : CommonDataModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    [AllowNull]
    public string? Description { get; set; }
}