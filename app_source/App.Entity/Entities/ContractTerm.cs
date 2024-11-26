using FS.Commons.Models;

namespace App.Entity.Entities;

public class ContractTerm : CommonDataModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long ContractId { get; set; }
}