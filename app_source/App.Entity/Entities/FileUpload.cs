using FS.Commons.Models;

namespace App.Entity.Entities;

public partial class FileUpload : CommonDataModel
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
}
