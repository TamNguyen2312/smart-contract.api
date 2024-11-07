using System.ComponentModel.DataAnnotations;
using FS.Common.Models.Models.Interfaces;
using FS.Commons;

namespace App.Entity.DTOs.Department;

public class DepartmentRequestDTO : IEntity<Entities.Department>
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = Constants.Required)]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    
    public Entities.Department GetEntity()
    {
        return new Entities.Department
        {
            Id = Id,
            Name = Name,
            Description = Description
        };
    }
}