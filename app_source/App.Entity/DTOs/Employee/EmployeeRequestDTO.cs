using FS.Common.Models.Models.Interfaces;

namespace App.Entity.DTOs.Employee;

public class EmployeeRequestDTO : IEntity<Entities.Employee>
{
    public long Id { get; set; }
    public string DepartmentName { get; set; } = null!;

    public Entities.Employee GetEntity()
    {
        return new Entities.Employee
        {
            Id = Id,
            DepartmentName = DepartmentName
        };
    }
}