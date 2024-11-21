using FS.Commons.Models;

namespace App.Entity.Entities;

public class CustomerDepartmentAssign : CommonDataModel
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public long DeparmentId { get; set; }
    public string? Description { get; set; }
}