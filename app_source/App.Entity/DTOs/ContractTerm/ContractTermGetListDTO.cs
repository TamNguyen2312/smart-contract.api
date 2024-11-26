using FS.Commons.Models;

namespace App.Entity.DTOs.ContractTerm;

public class ContractTermGetListDTO : PagingModel
{
    public DateRange CreatedDate { get; set; }    
    public DateRange ModifiedDate { get; set; }    
}