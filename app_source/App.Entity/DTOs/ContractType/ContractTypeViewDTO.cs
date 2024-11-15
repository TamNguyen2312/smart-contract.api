using System;

namespace App.Entity.DTOs.ContractType;

public class ContractTypeViewDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

}
