using App.Entity.DTOs.CustomerDeparmentAssign;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface ICustomerDepartmentAssignBizLogic
{
    Task<BaseResponse> CreateUpdateCustomerDepartmentAssgin(CustomerDepartmentAssignRequestDTO dto, long userId);
    Task<CustomerDepartmentAssignViewDTO> GetCustomerDepartmentAssign(long id);

    Task<List<CustomerDepartmentAssignViewDTO>> GetCustomerDepartmentAssignsByAdmin(
        CustomerDepartmentAssignGetListDTO dto,
        string userName);

    Task<List<CustomerDepartmentAssignViewDTO>> GetCustomerDepartmentAssignsByManager(
        CustomerDepartmentAssignGetListDTO dto, string managerId);

    Task<bool> ManagerHasAccessToAssignAsync(string managerId, long id);
}