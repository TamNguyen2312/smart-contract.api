using FS.Commons.Models;

namespace App.Entity.Entities;

public class SnapshotMetadata : CommonDataModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string Action { get; set; } = null!;
    public string StoredData { get; set; } = null!;
}