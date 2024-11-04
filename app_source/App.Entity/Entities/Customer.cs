namespace App.Entity.Entities;

public partial class Customer
{
    public long Id { get; set; } 
    public string CompanyName { get; set; } = null!;
    public string TaxIdentificationNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
}