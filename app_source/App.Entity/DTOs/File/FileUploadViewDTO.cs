using System;
using System.Reflection.Metadata;
using App.Entity.Entities;
using FS.Commons;

namespace App.Entity.DTOs.File;

public class FileUploadViewDTO
{
    public string FilePath { get; set; } = null!;
    public long UserId { get; set; }
    public string CreatedBy { get; set; } = null!;
    public string CreatedDate { get; set; } = null!;
    public string ModifiedBy { get; set; } = null!;
    public string ModifiedDate { get; set; } = null!;

    public FileUploadViewDTO(FileUpload fileUpload)
    {
        FilePath = Path.Combine(fileUpload.FilePath, fileUpload.FileName);
        UserId = fileUpload.UserId;
        CreatedBy = fileUpload.CreatedBy;
        CreatedDate = fileUpload.CreatedDate.HasValue ? fileUpload.CreatedDate.Value.ToString(Constants.FormatDate) : null;
        ModifiedBy = fileUpload.ModifiedBy;
        ModifiedDate = fileUpload.ModifiedDate.HasValue ? fileUpload.ModifiedDate.Value.ToString(Constants.FormatDate) : null;
    }
}
