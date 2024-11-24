using System.ComponentModel.DataAnnotations;
using App.Entity.Entities;
using FS.Commons;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.Contract;

public class ContractAssignRequestDTO : IEntity<ContractDepartmentAssign>
{
    [Required(ErrorMessage = Constants.Required)]
    public long ContractId { get; set; }
    [Required(ErrorMessage = Constants.Required)]
    public long DepartmentId { get; set; }

    public DateTime? EndDate { get; set; }
    
    public ContractDepartmentAssign GetEntity()
    {
        return new ContractDepartmentAssign
        {
            ContractId = ContractId,
            DepartmentId = DepartmentId,
            EndDate = EndDate
        };
    }
}