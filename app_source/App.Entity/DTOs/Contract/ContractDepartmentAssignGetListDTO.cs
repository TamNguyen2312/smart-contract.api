using FS.Commons.Models;

namespace App.Entity.DTOs.Contract;

public class ContractDepartmentAssignGetListDTO : PagingModel
{
    public DateRange CreatedDate { get; set; }
    public DateRange EndDate { get; set; }
    public int? ExpirationDaysLeft { get; set; }
    public bool IsExpried { get; set; }
}