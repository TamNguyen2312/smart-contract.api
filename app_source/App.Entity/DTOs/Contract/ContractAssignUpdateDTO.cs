using App.Entity.Entities;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.Contract;

public class ContractAssignUpdateDTO : ContractAssignRequestDTO
{
    public bool IsNow { get; set; }

    public new ContractDepartmentAssign GetEntity()
    {
        var entity = new ContractDepartmentAssign
        {
            ContractId = ContractId,
            DepartmentId = DepartmentId,
        };
        if (IsNow) entity.EndDate = DateTime.Now;
        else entity.EndDate = EndDate;
        return entity;
    }
}