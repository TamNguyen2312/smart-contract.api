using App.BLL.Interfaces;
using App.DAL.Interfaces;
using App.Entity.DTOs.CustomerDeparmentAssgin;
using FS.Commons.Models;
using FS.DAL.Interfaces;

namespace App.BLL.Implements;

public class CustomerDepartmentAssignBizLogic : ICustomerDepartmentAssignBizLogic
{
    private readonly ICustomerDepartmentAssignRepository _customerDepartmentAssignRepository;
    private readonly IIdentityRepository _identityRepository;

    public CustomerDepartmentAssignBizLogic(ICustomerDepartmentAssignRepository customerDepartmentAssignRepository, IIdentityRepository identityRepository)
    {
        _customerDepartmentAssignRepository = customerDepartmentAssignRepository;
        _identityRepository = identityRepository;
    }

    public async Task<BaseResponse> CreateUpdateCustomerDepartmentAssgin(CustomerDepartmentAssginRequestDTO dto, long userId)
    {
        var entity = dto.GetEntity();
        var user = await _identityRepository.GetByIdAsync(userId);
        var response = await _customerDepartmentAssignRepository.CreateUpdateCusomterDepartmentAssign(entity, user);
        return response;
    }
}