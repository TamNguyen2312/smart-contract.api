using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using FS.Commons;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.Contract;

public class ContractRequestDTO : IEntity<Entities.Contract>
{
    [Required(ErrorMessage = Constants.Required)]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = Constants.Required)]
    [DataType(DataType.Date)]
    public DateTime SignedDate { get; set; }


    [DataType(DataType.Date)]
    [Required(ErrorMessage = Constants.Required)]
    public DateTime EffectiveDate { get; set; }

    [DataType(DataType.Date)]
    [Required(ErrorMessage = Constants.Required)]
    public DateTime ExpirationDate { get; set; }

    [Required(ErrorMessage = Constants.Required)]
    public string KeyContent { get; set; } = null!;

    [Required(ErrorMessage = Constants.Required)]
    public string ContractFile { get; set; } = null!;
    
    [Required(ErrorMessage = Constants.Required)]
    public long CustomerId { get; set; }
    
    [Required(ErrorMessage = Constants.Required)]
    public long ContractTypeId { get; set; }
    
    public int? ContractDayLeft { get; set; }

    public int? AppendixDayLeft{ get; set; }
    
    
    public Entities.Contract GetEntity()
    {
        return new Entities.Contract
        {
            Title = Title,
            SignedDate = SignedDate,
            EffectiveDate = EffectiveDate,
            ExpirationDate = ExpirationDate,
            KeyContent = KeyContent,
            ContractFile = ContractFile,
            CustomerId = CustomerId,
            ContractTypeId = ContractTypeId,
            ContractDaysLeft = ContractDayLeft.HasValue ? ContractDayLeft.Value : 0,
            AppendixDaysLeft = AppendixDayLeft.HasValue ? AppendixDayLeft.Value : 0
        };
    }
}
