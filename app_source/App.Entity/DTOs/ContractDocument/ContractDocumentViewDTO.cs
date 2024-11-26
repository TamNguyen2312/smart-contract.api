using FS.Commons;

namespace App.Entity.DTOs.ContractDocument;

public class ContractDocumentViewDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? FileName { get; set; }
    public string? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }

    public ContractDocumentViewDTO(Entities.ContractDocument contractDocument)
    {
        Id = contractDocument.Id;
        Name = contractDocument.Name;
        Description = contractDocument.Description;
        FileName = contractDocument.FileName;
        CreatedDate = contractDocument.CreatedDate.HasValue
            ? contractDocument.CreatedDate.Value.ToString(Constants.FormatDate)
            : null;
        CreatedBy = contractDocument.CreatedBy;
        ModifiedDate = contractDocument.ModifiedDate.HasValue
            ? contractDocument.ModifiedDate.Value.ToString(Constants.FormatDate)
            : null;
        ModifiedBy = contractDocument.ModifiedBy;
    }
}