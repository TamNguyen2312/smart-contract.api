namespace FS.Commons.Models.DTOs;

public class PagingDataModel<T>
{
    public PagingDataModel(IEnumerable<T> listData, dynamic pagingModel)
    {
        ListObjects = listData;
        Paging = pagingModel;
    }
    public dynamic Paging { get; set; }
    public IEnumerable<T> ListObjects { get; set; }
}

public class PagingDataModel<T1, T2> where T2 : PagingModel
{
    public PagingDataModel(IEnumerable<T1> listData, T2 pagingModel)
    {
        ListObjects = listData;
        Paging = pagingModel;
    }
    public T2 Paging { get; set; }
    public IEnumerable<T1> ListObjects { get; set; }
}