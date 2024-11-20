using System.Diagnostics.CodeAnalysis;
using FS.Commons.Models;

namespace App.Entity.Entities;
public class Department : CommonDataModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public int EmployeeQuantity { get; set; }
    public int MornitorQuantity { get; set; }
    [AllowNull]
    public string? Description { get; set; }
}