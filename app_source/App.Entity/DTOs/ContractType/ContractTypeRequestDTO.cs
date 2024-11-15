using System.ComponentModel.DataAnnotations;
using FS.Commons;
using FS.Commons.Interfaces;
using App.Entity.Entities;

namespace App.Entity.DTOs.ContractType;

public class ContractTypeRequestDTO : IEntity<Entities.ContractType>
{
    public long Id { get; set; }
    [Required(ErrorMessage = Constants.Required)]
    public string Name { get; set; } = null!;

    public Entities.ContractType GetEntity()
    {
        return new Entities.ContractType
        {
            Id = Id,
            Name = Name
        };
    }
}
