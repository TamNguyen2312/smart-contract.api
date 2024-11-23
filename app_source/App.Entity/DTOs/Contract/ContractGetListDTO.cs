using FS.Commons.Models;

namespace App.Entity.DTOs.Contract;

public class ContractGetListDTO : PagingModel
{
    public long? CustomerId { get; set; }
    public long? ContractTypeId { get; set; }

    public DateRange SignedDate { get; set; } = new DateRange();
    public DateRange EffectiveDate { get; set; } = new DateRange();
    public DateRange ExpirationDate { get; set; } = new DateRange();
}