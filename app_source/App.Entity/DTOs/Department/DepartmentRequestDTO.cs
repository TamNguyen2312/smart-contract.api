using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FS.Commons;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.Department;

public class DepartmentRequestDto : IEntity<Entities.Department>
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = Constants.Required)]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }

    [Required(ErrorMessage = Constants.Required)]
    public int EmployeeQuantity { get; set; }
    
    [JsonIgnore]
    public int MornitorQuantity { get; set; }
    
    
    public Entities.Department GetEntity()
    {
        return new Entities.Department
        {
            Id = Id,
            Name = Name,
            Description = Description,
            EmployeeQuantity = EmployeeQuantity,
            MornitorQuantity = MornitorQuantity
        };
    }
    
}