using FS.Commons;

namespace App.Entity.DTOs.Contract;

public class ContractViewDTO
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string SignedDate { get; set; } = null!;
    public string EffectiveDate { get; set; } = null!;
    public string ExpirationDate { get; set; } = null!;
    public string KeyContent { get; set; } = null!;
    public string ContractFile { get; set; } = null!;
    public int ContractDaysLeft { get; set; }
    public int AppendixDaysLeft { get; set; }
    public string CustomerName { get; set; } = null!;
    public string ContractTypeName { get; set; } = null!;
    public string? CreatedBy { get; set; }
    public string? CreatedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public string? ModifiedDate { get; set; }

    public ContractViewDTO(Entities.Contract contract, Entities.Customer customer, Entities.ContractType contractType)
    {
        Id = contract.Id;
        Title = contract.Title;
        SignedDate = contract.SignedDate.ToString(Constants.FormatDate);
        EffectiveDate = contract.EffectiveDate.ToString(Constants.FormatDate);
        ExpirationDate = contract.ExpirationDate.ToString(Constants.FormatDate);
        KeyContent = contract.KeyContent;
        ContractFile = contract.ContractFile;
        ContractDaysLeft = contract.ContractDaysLeft;
        AppendixDaysLeft = contract.AppendixDaysLeft;
        CustomerName = customer.CompanyName;
        ContractTypeName = contractType.Name;
        CreatedBy = contract.CreatedBy;
        ModifiedBy = contract.ModifiedBy;
        CreatedDate = contract.CreatedDate.HasValue ? contract.CreatedDate.Value.ToString(Constants.FormatDate) : null;
        ModifiedDate = contract.ModifiedDate.HasValue ? contract.ModifiedDate.Value.ToString(Constants.FormatDate) : null;
    }
}
