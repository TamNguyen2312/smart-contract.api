using FS.Commons.Models;

namespace App.Entity.Entities;

public class ContractDocument : CommonDataModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? FileName { get; set; }
    public long ContractId { get; set; }
}