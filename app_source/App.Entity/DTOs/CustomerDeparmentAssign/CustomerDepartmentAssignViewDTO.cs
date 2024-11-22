using FS.Commons;

namespace App.Entity.DTOs.CustomerDeparmentAssign;

public class CustomerDepartmentAssignViewDTO
{
    public long Id { get; set; }
    
    public string CustomerName { get; set; }
    
    public string DeparmentName { get; set; }
    
    public string? Description { get; set; }

    public string? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }

    public CustomerDepartmentAssignViewDTO(Entities.CustomerDepartmentAssign customerDeparmentAssign, Entities.Customer customer, Entities.Department department)
    {
        Id = customerDeparmentAssign.Id;
        CustomerName = customer.CompanyName;
        DeparmentName = department.Name;
        Description = customerDeparmentAssign.Description;
        CreatedDate = customerDeparmentAssign.CreatedDate.HasValue
            ? customerDeparmentAssign.CreatedDate.Value.ToString(Constants.FormatDate)
            : null;
        CreatedBy = customerDeparmentAssign.CreatedBy;
        ModifiedDate = customerDeparmentAssign.ModifiedDate.HasValue
            ? customerDeparmentAssign.ModifiedDate.Value.ToString(Constants.FormatDate)
            : null;
        ModifiedBy = customerDeparmentAssign.ModifiedBy;
    }
}