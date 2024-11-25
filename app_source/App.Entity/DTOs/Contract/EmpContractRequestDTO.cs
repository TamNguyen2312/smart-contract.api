using System.ComponentModel.DataAnnotations;
using App.Entity.Entities;
using FS.Commons;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.Contract;

public class EmpContractRequestDTO : IEntity<EmpContract>
{
    [Required(ErrorMessage = Constants.Required)]
    public string EmployeeId { get; set; }
    [Required(ErrorMessage = Constants.Required)]
    public long ContractId { get; set; }

    public string? Description { get; set; }
    
    
    public EmpContract GetEntity()
    {
        return new EmpContract
        {
            EmployeeId = EmployeeId,
            ContractId = ContractId,
            Description = Description
        };
    }
}