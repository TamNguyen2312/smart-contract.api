using System.ComponentModel.DataAnnotations;
using FS.Commons;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.ContractTerm;

public class ContractTermRequestDTO : IEntity<Entities.ContractTerm>
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = Constants.Required)]
    public string Name { get; set; } = null!;
    
    [Required(ErrorMessage = Constants.Required)]
    public string Description { get; set; } = null!;
    
    [Required(ErrorMessage = Constants.Required)]
    public long ContractId { get; set; }

    public Entities.ContractTerm GetEntity()
    {
        return new Entities.ContractTerm
        {
            Id = Id,
            Name = Name,
            Description = Description,
            ContractId = ContractId
        };
    }
}