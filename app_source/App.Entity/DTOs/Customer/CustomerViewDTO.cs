using FS.Commons;
using FS.Commons.Models.DTOs;

namespace App.Entity.DTOs.Customer;

public class CustomerViewDTO
{
    public long Id { get; set; }
    public string CompanyName { get; set; } = null!;
    public string TaxIdentificationNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? CreatedBy { get; set; }
    public string? CreatedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public string? ModifiedDate { get; set; }

    public CustomerViewDTO(Entities.Customer customer)
    {
        Id = customer.Id;
        CompanyName = customer.CompanyName;
        TaxIdentificationNumber = customer.TaxIdentificationNumber;
        Email = customer.Email;
        PhoneNumber = customer.PhoneNumber;
        Address = customer.Address;
        CreatedBy = customer.CreatedBy;
        CreatedDate = customer.CreatedDate.HasValue ? customer.CreatedDate.Value.ToString(Constants.FormatDate) : null;
        ModifiedBy = customer.ModifiedBy;
        ModifiedDate = customer.ModifiedDate.HasValue ? customer.ModifiedDate.Value.ToString(Constants.FormatDate) : null;
    }
}