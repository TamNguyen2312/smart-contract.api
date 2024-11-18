using FS.Commons.Models;

namespace App.Entity.Entities;

public class Contract : CommonDataModel
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime SignedDate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string KeyContent { get; set; } = null!;
    public string ContractFile { get; set; } = null!;
    public long CustomerId { get; set; }
    public long ContractTypeId { get; set; }

}
