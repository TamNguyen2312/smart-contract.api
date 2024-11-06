namespace FS.Commons.Models;

public class PagingModel
{
    public int PageIndex { get; set; }
    public int  PageSize { get; set; }
    public string? Keyword { get; set; }
    [OutputParam]
    public int TotalRecord { get; set; }
    public int Day { get; set; }
    public int Week { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalRecord * 1f / PageSize);
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
    public string CreatedBy { get; set; }

    public static PagingModel Default = new PagingModel()
    {
        PageIndex = 1,
        PageSize = 10
    };
}