using FS.Commons.Models;

namespace App.Entity.Entities;

public class EmpContract : CommonDataModel
{
    public string EmployeeId { get; set; } = null!;
    public long ContractId { get; set; }
    public string? Description { get; set; }
}