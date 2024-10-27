using System;
using System.Text.Json.Serialization;

namespace FS.Commons.Models;

public class BaseTokenModel
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    [JsonIgnore]
    public long userId { get; set; }
}
