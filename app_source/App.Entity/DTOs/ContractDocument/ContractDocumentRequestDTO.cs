using System.ComponentModel.DataAnnotations;
using FS.Commons;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.ContractDocument;

public class ContractDocumentRequestDTO : IEntity<Entities.ContractDocument>
{
    public long Id { get; set; }
    [Required(ErrorMessage = Constants.Required)]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = Constants.Required)]
    public string Description { get; set; } = null!;
    public string? FileName { get; set; }
    
    [Required(ErrorMessage = Constants.Required)]
    public long ContractId { get; set; }
    
    
    public Entities.ContractDocument GetEntity()
    {
        return new Entities.ContractDocument
        {
            Id = Id,
            Name = Name,
            Description = Description,
            FileName = FileName,
            ContractId = ContractId
        };
    }
}