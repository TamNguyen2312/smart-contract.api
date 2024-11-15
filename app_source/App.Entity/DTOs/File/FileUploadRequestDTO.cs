using System;
using App.Entity.Entities;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.File;

public class FileUploadRequestDTO : IEntity<FileUpload>
{
    public long Id { get; set; }
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public long UserId { get; set; }

    public FileUpload GetEntity()
    {
        return new FileUpload
        {
            Id = Id,
            FileName = FileName,
            FilePath = FilePath,
            UserId = UserId
        };
    }
}
