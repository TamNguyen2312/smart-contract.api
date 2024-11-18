using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using FS.Commons;

namespace App.Entity.DTOs.Contract;

public class ContractRequestDTO
{
    [Required(ErrorMessage = Constants.Required)]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = Constants.Required)]
    [DataType(DataType.Date)]
    public DateTime SignedDate { get; set; }


    [DataType(DataType.Date)]
    [Required(ErrorMessage = Constants.Required)]
    public DateTime EffectiveDate { get; set; }


    [DataType(DataType.Date)]
    [Required(ErrorMessage = Constants.Required)]
    public DateTime ExpirationDate { get; set; }

    [Required(ErrorMessage = Constants.Required)]
    public string KeyContent { get; set; } = null!;

    [Required(ErrorMessage = Constants.Required)]
    public string ContractFile { get; set; } = null!;

    [Required(ErrorMessage = Constants.Required)]
    public long CustomerId { get; set; }

    [Required(ErrorMessage = Constants.Required)]
    public long ContractTypeId { get; set; }
}
