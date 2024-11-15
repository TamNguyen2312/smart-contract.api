using System;
using System.Reflection.Metadata;
using FS.Commons;

namespace App.Entity.DTOs.ContractType;

public class ContractTypeViewDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? CreatedBy { get; set; }
    public string? CreatedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public string? ModifiedDate { get; set; }


    public ContractTypeViewDTO(Entities.ContractType contractType)
    {
        Id = contractType.Id;
        Name = contractType.Name;
        CreatedBy = contractType.CreatedBy;
        CreatedDate = contractType.CreatedDate.HasValue ? contractType.CreatedDate.Value.ToString(Constants.FormatDate) : null;
        ModifiedBy = contractType.ModifiedBy;
        ModifiedDate = contractType.ModifiedDate.HasValue ? contractType.ModifiedDate.Value.ToString(Constants.FormatDate) : null;
    }
}
