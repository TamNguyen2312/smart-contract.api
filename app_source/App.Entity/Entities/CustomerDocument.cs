using FS.Commons.Models;

namespace App.Entity.Entities;

public class CustomerDocument : CommonDataModel
{
    public long Id { get; set; }
    public string Description { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public long CustomerId { get; set; }
}