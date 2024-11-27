using FS.Commons;

namespace App.Entity.DTOs.ContractAppendix;

public class ContractAppendixViewDTO
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string SignedDate { get; set; } = null!;
    public string EffectiveDate { get; set; } = null!;
    public string ExpirationDate { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? FileName { get; set; }
    public string? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }

    public ContractAppendixViewDTO(Entities.ContractAppendix contractAppendix)
    {
        Id = contractAppendix.Id;
        Title = contractAppendix.Title;
        SignedDate = contractAppendix.SignedDate.ToString(Constants.FormatDate);
        EffectiveDate = contractAppendix.EffectiveDate.ToString(Constants.FormatDate);
        ExpirationDate = contractAppendix.ExpirationDate.ToString(Constants.FormatDate);
        ;
        Content = contractAppendix.Content;
        FileName = contractAppendix.FileName;
        CreatedDate = contractAppendix.CreatedDate.HasValue
            ? contractAppendix.CreatedDate.Value.ToString(Constants.FormatDate)
            : null;
        CreatedBy = contractAppendix.CreatedBy;
        ModifiedDate = contractAppendix.ModifiedDate.HasValue
            ? contractAppendix.ModifiedDate.Value.ToString(Constants.FormatDate)
            : null;
        ModifiedBy = contractAppendix.ModifiedBy;
    }
}