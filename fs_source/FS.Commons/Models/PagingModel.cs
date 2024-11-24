using System.ComponentModel.DataAnnotations;

namespace FS.Commons.Models;

public class PagingModel
{
    [Required(ErrorMessage = Constants.Required)]
    public int PageIndex { get; set; }
    [Required(ErrorMessage = Constants.Required)]
    public int PageSize { get; set; }
    public string? Keyword { get; set; }
    public OrderDate? OrderDate { get; set; }
    [OutputParam]
    public int TotalRecord { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalRecord * 1f / PageSize);
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static PagingModel Default = new PagingModel()
    {
        PageIndex = 1,
        PageSize = 10
    };


    #region Filter
    public bool IsValidOrderDate()
    {
        if (OrderDate.HasValue)
            return Enum.IsDefined(typeof(OrderDate), OrderDate);
        return true;
    }
    #endregion
}

public enum OrderDate
{
    DescesdendingCreated = 1,
    IncreasingCreated = 2,
    DescendingModified = 3,
    IncreasingModified = 4
}
