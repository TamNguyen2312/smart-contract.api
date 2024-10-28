using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FS.Commons.Models;

public class BaseTokenModel
{
    [Required(ErrorMessage = Constants.Required)]
    public string AccessToken { get; set; } = null!;
    [Required(ErrorMessage = Constants.Required)]
    public string RefreshToken { get; set; } = null!;
    [JsonIgnore]
    public long userId { get; set; }
}
