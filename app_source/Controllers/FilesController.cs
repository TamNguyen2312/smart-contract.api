using App.API.Filter;
using App.Entity.DTOs.File;
using FS.BaseAPI;
using FS.Commons;
using FS.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class FilesController : BaseAPIController
    {
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly long _imageFileSizeLimit = 10 * 1024 * 1024;
        private readonly long _fileSizeLimit = 50 * 1024 * 1024;

        public FilesController(IHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        [FSAuthorize]
        [HttpPost()]
        [Route("upload-customize-photo")]
        public async Task<IActionResult> UploadCustomizePhoto(FileUploadDTO dto)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await dto.File.CopyToAsync(memoryStream);
                    var extension = Path.GetExtension(dto.File.FileName);

                    var isSvg = extension.Equals(".svg", StringComparison.OrdinalIgnoreCase);
                    var isValid = IsValidFileExtension(dto.File.FileName, new string[] { ".jpg", ".png", ".svg", ".jpeg", ".dng" });
                    if (!isValid)
                    {
                        ModelState.AddModelError("File", "Không hỗ trợ định dạng ảnh hiện tại.");
                        return ModelInvalid();
                    }

                    if (dto.File.Length > _imageFileSizeLimit)
                    {
                        var megabyteSizeLimit = _imageFileSizeLimit / 1048576;
                        ModelState.AddModelError("File", $"Kích thước ảnh vượt quá quy định cho phép ({megabyteSizeLimit:N1} MB).");
                        return ModelInvalid();
                    }

                    if (!ModelState.IsValid) return ModelInvalid();


                    string safeFileName = string.Empty;
                    if (!string.IsNullOrEmpty(dto.CustomFileName))
                    {
                        dto.ProccessFileName(isCamelCase: true);
                        safeFileName = dto.CustomFileName;
                    }
                    else
                    {
                        safeFileName = Path.GetFileNameWithoutExtension(dto.File.FileName);
                    }

                    var guidFileName = $"{safeFileName}_{Guid.NewGuid()}{extension}";

                    string imageFolder = string.Empty;
                    if (IsAdmin)
                        imageFolder = "admin";
                    else if (IsManager)
                        imageFolder = "manager";
                    else if (IsEmployee)
                        imageFolder = "employee";
                    else
                        imageFolder = "others";

                    string subFolder = @$"userId_{UserId}";

                    // Construct the folder path
                    var guildStringPath = new string[]
                    {
                        "images",
                        imageFolder,
                        subFolder,
                        $"{guidFileName}{extension}"
                    };

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Helpers.PathCombine(guildStringPath));

                    string directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    using (var fileStream = System.IO.File.Create(path))
                    {
                        await fileStream.WriteAsync(memoryStream.ToArray());
                        fileStream.Close();
                    }

                    string cdnhost = _configuration.GetSection("AppSettings").GetValue<string>("CdnUrl");
                    string imageUrl = $"{cdnhost}{Helpers.UrlCombine(guildStringPath)}";
                    string thumbUrl = isSvg ? imageUrl : $"{cdnhost}{CompressThumbnailWithNew(guildStringPath, path)}";

                    return SaveSuccess(new
                    {
                        Success = true,
                        ImageUrl = imageUrl,
                        ThumbnailUrl = thumbUrl
                    });
                }
            }
            catch (Exception ex)
            {
                // _logger.LogError("UploadStockPhoto: {0} {1}", ex.Message, ex.StackTrace);
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }


        [FSAuthorize]
        [HttpPost()]
        [Route("upload-customize-file")]
        public async Task<IActionResult> UploadCustomizeFile(FileUploadDTO dto)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await dto.File.CopyToAsync(memoryStream);
                    var extension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();

                    var allowedExtensions = new string[] { ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".txt" };
                    var isValid = IsValidFileExtension(dto.File.FileName, allowedExtensions);
                    if (!isValid)
                    {
                        ModelState.AddModelError("File", "Không hỗ trợ định dạng tệp hiện tại.");
                        return ModelInvalid();
                    }

                    if (dto.File.Length > _fileSizeLimit)
                    {
                        var megabyteSizeLimit = _fileSizeLimit / 1048576;
                        ModelState.AddModelError("File", $"Kích thước tệp vượt quá giới hạn cho phép ({megabyteSizeLimit:N1} MB).");
                        return ModelInvalid();
                    }

                    if (!ModelState.IsValid) return ModelInvalid();

                    string safeFileName = string.Empty;
                    if (!string.IsNullOrEmpty(dto.CustomFileName))
                    {
                        dto.ProccessFileName(isCamelCase: true);
                        safeFileName = dto.CustomFileName;
                    }
                    else
                    {
                        safeFileName = Path.GetFileNameWithoutExtension(dto.File.FileName);
                    }

                    var guidFileName = $"{safeFileName}_{Guid.NewGuid()}{extension}";

                    // Xác định thư mục lưu trữ dựa trên vai trò
                    string fileFolder = string.Empty;
                    if (IsAdmin)
                        fileFolder = "admin";
                    else if (IsManager)
                        fileFolder = "manager";
                    else if (IsEmployee)
                        fileFolder = "employee";
                    else
                        fileFolder = "others";

                    string subFolder = @$"userId_{UserId}";

                    var guildStringPath = new string[]
                    {
                        "files",
                        fileFolder,
                        subFolder,
                        guidFileName
                    };

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Helpers.PathCombine(guildStringPath));

                    string directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    using (var fileStream = System.IO.File.Create(path))
                    {
                        await fileStream.WriteAsync(memoryStream.ToArray());
                        fileStream.Close();
                    }

                    string cdnhost = _configuration.GetSection("AppSettings").GetValue<string>("CdnUrl");
                    string fileUrl = $"{cdnhost}{Helpers.UrlCombine(guildStringPath)}";

                    return SaveSuccess(new
                    {
                        Success = true,
                        FileUrl = fileUrl
                    });
                }
            }
            catch (Exception ex)
            {
                // _logger.LogError("UploadStockPhoto: {0} {1}", ex.Message, ex.StackTrace);
                ConsoleLog.WriteExceptionToConsoleLog(ex);
                return Error(Constants.SomeThingWentWrong);
            }
        }


        #region PRIVATE

        private bool IsValidFileExtension(string fileName, string[] allowedExtensions)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        private string CompressThumbnailWithNew(string imagePath)
        {

            var thumbnailFileName = Path.GetFileNameWithoutExtension(imagePath) + "_thumb" + Path.GetExtension(imagePath);
            var thumbnailPath = Path.Combine(Path.GetDirectoryName(imagePath), thumbnailFileName);


            return thumbnailPath;
        }

        private string CompressThumbnailWithNew(string[] guildStringPath, string originalImagePath)
        {
            // Create the thumbnail file name
            var originalFileName = guildStringPath.Last();
            var thumbnailFileName = Path.GetFileNameWithoutExtension(originalFileName) + "_thumb" + Path.GetExtension(originalFileName);

            // Replace the original file name with the thumbnail file name
            var thumbnailGuildStringPath = guildStringPath.Take(guildStringPath.Length - 1)
                                                          .Concat(new[] { thumbnailFileName })
                                                          .ToArray();

            // Get the physical path for the thumbnail
            var thumbnailPhysicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Helpers.PathCombine(thumbnailGuildStringPath));

            // Ensure the directory exists
            string directory = Path.GetDirectoryName(thumbnailPhysicalPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Create the thumbnail (implement your thumbnail creation logic here)
            using (var image = Image.Load(originalImagePath))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(150, 0)
                }));
                image.Save(thumbnailPhysicalPath);
            }

            // Return the URL path to the thumbnail
            var thumbnailUrlPath = Helpers.UrlCombine(thumbnailGuildStringPath);
            return thumbnailUrlPath;
        }

        #endregion
    }
}