using App.Entity.Entities;
using FS.Commons;

namespace App.Entity.DTOs.Contract;

public class EmpContractViewDTO
{
    public string EmployeeId { get; set; }
    public long ContractId { get; set; }
    public string? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }

    public EmpContractViewDTO(EmpContract empContract)
    {
        EmployeeId = empContract.EmployeeId;
        ContractId = empContract.ContractId;
        CreatedDate = empContract.CreatedDate.HasValue ? empContract.CreatedDate.Value.ToString(Constants.FormatDate) : null;
        CreatedBy = empContract.CreatedBy;
    }
}