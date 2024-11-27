using FS.Commons.Models;

namespace App.Entity.DTOs.ContractAppendix;

public class ContractAppendixGetListDTO : PagingModel
{
    public DateRange SignedDate { get; set; }
    public DateRange EffectiveDate { get; set; }
    public DateRange ExpirationDate { get; set; }
    public DateRange CreatedDate { get; set; }
    public DateRange ModifiedDate { get; set; }
}