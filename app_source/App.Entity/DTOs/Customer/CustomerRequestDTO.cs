using System.ComponentModel.DataAnnotations;
using App.Entity.Mappers;
using AutoMapper;
using FS.Common.Models.Models.Interfaces;
using FS.Commons;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.Customer;

public class CustomerRequestDto : IEntity<Entities.Customer>
{
    public long Id { get; set; } 
    
    [Required(ErrorMessage = Constants.Required)]
    public string CompanyName { get; set; } = null!;
    
    public string TaxIdentificationNumber { get; set; } = null!;
    
    
    [Required(ErrorMessage = Constants.Required)]
    [Display(Name = "Email"), StringLength(255, ErrorMessage = Constants.MaxlengthError)]
    [EmailAddress(ErrorMessage = Constants.EmailAddressFormatError)]
    public string Email { get; set; } = null!;
    
    
    [Required(ErrorMessage = Constants.Required)]
    [StringLength(11, ErrorMessage = "Số điện thoại chỉ chứa tối đa 11 số!")]
    public string PhoneNumber { get; set; } = null!;
    
    public string Address { get; set; } = null!;
    
    public Entities.Customer GetEntity()
    {
        return new Entities.Customer
        {
            Id = Id,
            CompanyName = CompanyName,
            TaxIdentificationNumber = TaxIdentificationNumber,
            Email = Email,
            PhoneNumber = PhoneNumber,
            Address = Address
        };
    }
    
}