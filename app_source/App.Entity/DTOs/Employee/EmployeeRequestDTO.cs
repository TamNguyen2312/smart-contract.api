using FS.Common.Models.Models.Interfaces;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.Employee;

public class EmployeeRequestDTO : IEntity<Entities.Employee>
{
    public string Id { get; set; } = null!;
    public long DepartmentId { get; set; }

    public Entities.Employee GetEntity()
    {
        return new Entities.Employee
        {
            Id = Id,
            DepartmentId = DepartmentId
        };
    }
}