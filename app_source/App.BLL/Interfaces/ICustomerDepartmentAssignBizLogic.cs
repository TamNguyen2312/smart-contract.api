using App.Entity.DTOs.CustomerDeparmentAssign;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface ICustomerDepartmentAssignBizLogic
{
    Task<BaseResponse> CreateUpdateCustomerDepartmentAssgin(CustomerDepartmentAssginRequestDTO dto, long userId);
    Task<CustomerDepartmentAssignViewDTO> GetCustomerDepartmentAssign(long id);
}