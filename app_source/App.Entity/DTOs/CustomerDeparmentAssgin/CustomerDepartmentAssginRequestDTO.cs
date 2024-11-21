using System.ComponentModel.DataAnnotations;
using App.Entity.Entities;
using FS.Commons;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.CustomerDeparmentAssgin;

public class CustomerDepartmentAssginRequestDTO : IEntity<Entities.CustomerDepartmentAssign>
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = Constants.Required)]
    public long CustomerId { get; set; }
    
    [Required(ErrorMessage = Constants.Required)]
    public long DeparmentId { get; set; }
    
    public string? Description { get; set; }
    
    public CustomerDepartmentAssign GetEntity()
    {
        return new CustomerDepartmentAssign
        {
            Id = Id,
            CustomerId = CustomerId,
            DeparmentId = DeparmentId,
            Description = Description
        };
    }
}