using System.ComponentModel.DataAnnotations;
using FS.Commons;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.Contract;

public class ContractUpdateDTO : ContractRequestDTO, IEntity<Entities.Contract>
{
    [Required(ErrorMessage = Constants.Required)]
    public long Id { get; set; }


    public new Entities.Contract GetEntity()
    {
        return new Entities.Contract
        {
            Id = Id,
            Title = Title,
            SignedDate = SignedDate,
            EffectiveDate = EffectiveDate,
            ExpirationDate = ExpirationDate,
            KeyContent = KeyContent,
            ContractFile = ContractFile,
            CustomerId = base.CustomerId,
            ContractTypeId = base.ContractTypeId,
            ContractDaysLeft = ContractDayLeft.HasValue ? ContractDayLeft.Value : 0,
            AppendixDaysLeft = AppendixDayLeft.HasValue ? AppendixDayLeft.Value : 0
        };
    }
}