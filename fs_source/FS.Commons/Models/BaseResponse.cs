using System;

namespace FS.Commons.Models;

public class BaseResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}

public class BaseResponse<T> : BaseResponse where T : class
{
    public T? Data { get; set; }
}
