using System.Security.Principal;
using FS.Commons.Models;

namespace App.Entity.Entities;

public class ContractAppendix : CommonDataModel
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime SignedDate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Content { get; set; } = null!;
    public string? FileName { get; set; }
    public long ContractId { get; set; }
}