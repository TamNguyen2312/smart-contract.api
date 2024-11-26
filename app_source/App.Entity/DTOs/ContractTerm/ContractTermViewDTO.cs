using FS.Commons;

namespace App.Entity.DTOs.ContractTerm;

public class ContractTermViewDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? FileName { get; set; }
    public string? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }

    public ContractTermViewDTO(Entities.ContractTerm contractTerm)
    {
        Id = contractTerm.Id;
        Name = contractTerm.Name;
        Description = contractTerm.Description;
        CreatedDate = contractTerm.CreatedDate.HasValue
            ? contractTerm.CreatedDate.Value.ToString(Constants.FormatDate)
            : null;
        CreatedBy = contractTerm.CreatedBy;
        ModifiedDate = contractTerm.ModifiedDate.HasValue
            ? contractTerm.ModifiedDate.Value.ToString(Constants.FormatDate)
            : null;
        ModifiedBy = contractTerm.ModifiedBy;
    }
}