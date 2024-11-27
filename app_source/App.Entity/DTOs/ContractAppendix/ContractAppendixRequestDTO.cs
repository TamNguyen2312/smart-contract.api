using System.ComponentModel.DataAnnotations;
using FS.Commons;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.ContractAppendix;

public class ContractAppendixRequestDTO : IEntity<Entities.ContractAppendix>
{
    public long Id { get; set; }
    [Required(ErrorMessage = Constants.Required)]
    public string Title { get; set; } = null!;
    
    [Required(ErrorMessage = Constants.Required)]
    [DataType(DataType.Date)]
    public DateTime SignedDate { get; set; }
    
    [Required(ErrorMessage = Constants.Required)]
    [DataType(DataType.Date)]
    public DateTime EffectiveDate { get; set; }
    
    [Required(ErrorMessage = Constants.Required)]
    [DataType(DataType.Date)]
    public DateTime ExpirationDate { get; set; }
    
    [Required(ErrorMessage = Constants.Required)]
    public string Content { get; set; } = null!;
    
    public string? FileName { get; set; }
    
    public long ContractId { get; set; }
    
    
    public Entities.ContractAppendix GetEntity()
    {
        return new Entities.ContractAppendix
        {
            Id = Id,
            Title = Title,
            SignedDate = SignedDate,
            EffectiveDate = EffectiveDate,
            ExpirationDate = ExpirationDate,
            Content = Content,
            FileName = FileName,
            ContractId = ContractId
        };
    }
}