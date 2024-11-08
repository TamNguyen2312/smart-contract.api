using FS.Common.Models.Models.Interfaces;

namespace App.Entity.DTOs.Manager;

public class ManagerRequestDTO : IEntity<Entities.Manager>
{
    public string Id { get; set; } //ref to userId
    public long DepartmentId { get; set; }
    
    
    public Entities.Manager GetEntity()
    {
        return new Entities.Manager
        {
            Id = Id,
            DepartmentId = DepartmentId
        };
    }
}