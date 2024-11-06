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
    public DateTime? CreatedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public UserViewDTO UserViewDto { get; set; }

    public CustomerViewDTO(Entities.Customer customer,UserViewDTO userViewDto)
    {
        Id = customer.Id;
        CompanyName = customer.CompanyName;
        TaxIdentificationNumber = customer.TaxIdentificationNumber;
        Email = customer.Email;
        PhoneNumber = customer.PhoneNumber;
        Address = customer.Address;
        CreatedBy = customer.CreatedBy;
        CreatedDate = customer.CreatedDate;
        ModifiedBy = customer.ModifiedBy;
        ModifiedDate = customer.ModifiedDate;
        UserViewDto = userViewDto;
    }
}