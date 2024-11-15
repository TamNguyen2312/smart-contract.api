using System;
using System.ComponentModel.DataAnnotations;
using FS.Commons;
using FS.Utility;
using Microsoft.AspNetCore.Http;

namespace App.Entity.DTOs.File;

public class FileUploadDTO
{
    [Required(ErrorMessage = Constants.Required)]
    public IFormFile File { get; set; } = null!;

    [StringLength(20, ErrorMessage = "Tên file không được vượt quá 20 kí tự")]
    public string? FileName { get; set; }

    public void ProccessFileName(bool isCamelCase = false)
    {
        if (!string.IsNullOrEmpty(FileName))
        {
            FileName = Helpers.ConvertToPascalOrCamelCase(FileName, isCamelCase);
        }
    }
}
