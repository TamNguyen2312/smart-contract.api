using FS.Commons;

namespace App.Entity.DTOs.CustomerDocument;

public class CustomerDocumentViewDTO
{
    public long Id { get; set; }
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public long CustomerId { get; set; }
    public string? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }

    public CustomerDocumentViewDTO(Entities.CustomerDocument customerDocument)
    {
        Id = customerDocument.Id;
        Description = customerDocument.Description;
        CustomerId = customerDocument.CustomerId;
        CreatedDate = customerDocument.CreatedDate.HasValue ? customerDocument.CreatedDate.Value.ToString(Constants.FormatDate) : null;
        CreatedBy = customerDocument.CreatedBy;
        ModifiedDate = customerDocument.ModifiedDate.HasValue ? customerDocument.ModifiedDate.Value.ToString(Constants.FormatDate) : null;
        ModifiedBy = customerDocument.ModifiedBy;
        FilePath = customerDocument.FilePath;
    }
}