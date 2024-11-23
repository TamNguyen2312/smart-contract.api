using FS.Commons.Models;

namespace App.Entity.Entities;

public class ContractDepartmentAssign : CommonDataModel
{
    public long ContractId { get; set; }
    public long DepartmentId { get; set; }
}