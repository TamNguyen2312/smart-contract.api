using FS.Commons.Models;

namespace App.Entity.DTOs.Contract;

public class EmpContractGetListDTO : PagingModel
{
    public string? EmployeeId { get; set; }
    public long? ContractId { get; set; }
    public DateRange CreatedDate { get; set; }
}