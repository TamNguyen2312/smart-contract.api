using FS.Commons.Models;

namespace App.Entity.DTOs.CustomerDocument;

public class CustomerDocumentGetListDTO : PagingModel
{
    public long? CustomerId { get; set; }
}