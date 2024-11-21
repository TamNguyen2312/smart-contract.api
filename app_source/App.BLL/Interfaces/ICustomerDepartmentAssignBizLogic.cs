using App.Entity.DTOs.CustomerDeparmentAssgin;
using FS.Commons.Models;

namespace App.BLL.Interfaces;

public interface ICustomerDepartmentAssignBizLogic
{
    Task<BaseResponse> CreateUpdateCustomerDepartmentAssgin(CustomerDepartmentAssginRequestDTO dto, long userId);
}