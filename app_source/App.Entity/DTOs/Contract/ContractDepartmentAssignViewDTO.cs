using App.Entity.Entities;
using FS.Commons;

namespace App.Entity.DTOs.Contract;

public class ContractDepartmentAssignViewDTO
{
    public long ContractId { get; set; }
    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public string? EndDate { get; set; }
    public string? CreatedDate { get; set; }
    public string CreatedBy { get; set; }

    public ContractDepartmentAssignViewDTO(ContractDepartmentAssign contractDepartmentAssign,
        Entities.Department department)
    {
        ContractId = contractDepartmentAssign.ContractId;
        DepartmentId = contractDepartmentAssign.DepartmentId;
        DepartmentName = department.Name;
        EndDate = contractDepartmentAssign.EndDate.HasValue
            ? contractDepartmentAssign.EndDate.Value.ToString(Constants.FormatDate)
            : null;
        CreatedDate = contractDepartmentAssign.CreatedDate.HasValue
            ? contractDepartmentAssign.CreatedDate.Value.ToString(Constants.FormatDate)
            : null;
        CreatedBy = contractDepartmentAssign.CreatedBy;
    }
}