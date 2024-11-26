using FS.Commons.Models;

namespace App.Entity.DTOs.ContractDocument;

public class ContractDocumentGetListDTO : PagingModel
{
    public DateRange CreatedDate { get; set; }
    public DateRange ModifiedDate { get; set; }
}