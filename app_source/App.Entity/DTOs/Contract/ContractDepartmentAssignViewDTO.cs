using App.Entity.Entities;

namespace App.Entity.DTOs.Contract;

public class ContractDepartmentAssignViewDTO
{
    public long ContractId { get; set; }
    public long DepartmentId { get; set; }
    public string DepartmentName{ get; set; }

    public ContractDepartmentAssignViewDTO(ContractDepartmentAssign contractDepartmentAssign, Entities.Department department)
    {
        ContractId = contractDepartmentAssign.ContractId;
        DepartmentId = contractDepartmentAssign.DepartmentId;
        DepartmentName = department.Name;
    }
}