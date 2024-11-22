using App.API.Filter;
using App.BLL.Interfaces;
using App.Entity.DTOs.Customer;
using App.Entity.DTOs.CustomerDeparmentAssign;
using FS.BaseAPI;
using FS.Commons;
using FS.Commons.Models.DTOs;
using FS.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class CustomerDepartmentAssignsController : BaseAPIController
    {
        private readonly ICustomerDepartmentAssignBizLogic _customerDepartmentAssignBizLogic;
        private readonly ICustomerBizLogic _customerBizLogic;
        private readonly IDepartmentBizLogic _departmentBizLogic;
        private readonly ILogger<CustomerDepartmentAssignsController> _logger;

        public CustomerDepartmentAssignsController(ICustomerDepartmentAssignBizLogic customerDepartmentAssignBizLogic,
            ICustomerBizLogic customerBizLogic,
            IDepartmentBizLogic departmentBizLogic,
            ILogger<CustomerDepartmentAssignsController> logger)
        {
            _customerDepartmentAssignBizLogic = customerDepartmentAssignBizLogic;
            _customerBizLogic = customerBizLogic;
            _departmentBizLogic = departmentBizLogic;
            _logger = logger;
        }

        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [Route("create-update-customer-department-assign")]
        public async Task<IActionResult> CreateUpdateCustomerDepartmentAssign(CustomerDepartmentAssignRequestDTO dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();


                var existedCustomer = await _customerBizLogic.GetCustomer(dto.CustomerId);
                if (existedCustomer == null)
                {
                    return SaveError("Khách hàng không tồn tại.");
                }

                var existedDepartment = await _departmentBizLogic.GetDepartment(dto.DeparmentId);
                if (existedDepartment == null)
                {
                    return SaveError("Phòng ban không tồn tại.");
                }

                var responnse =
                    await _customerDepartmentAssignBizLogic.CreateUpdateCustomerDepartmentAssgin(dto, UserId);
                if (!responnse.IsSuccess) return SaveError(responnse.Message);
                return SaveSuccess(responnse);
            }
            catch (Exception e)
            {
                _logger.LogError("CreateUpdateCustomerDepartmentAssign {0} {1}", e.Message, e.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        
        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [Route("get-customer-department-assigns-by-admin")]
        public async Task<IActionResult> GetCustomerDepartmentAssignsByAdmin(CustomerDepartmentAssignGetListDTO dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();


                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ.");
                    return ModelInvalid();
                }

                var data = await _customerDepartmentAssignBizLogic.GetCustomerDepartmentAssignsByAdmin(dto, UserName);
                var responnse = new PagingDataModel<CustomerDepartmentAssignViewDTO>(data, dto);
                return GetSuccess(responnse);
            }
            catch (Exception e)
            {
                _logger.LogError("GetCustomerDepartmentAssignsByAdmin {0} {1}", e.Message, e.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        
        [FSAuthorize(Policy = "ManagerRolePolicy")]
        [HttpPost]
        [Route("get-customer-department-assigns-by-manager")]
        public async Task<IActionResult> GetCustomerDepartmentAssignsByManager(CustomerDepartmentAssignGetListDTO dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();


                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ.");
                    return ModelInvalid();
                }

                var data = await _customerDepartmentAssignBizLogic.GetCustomerDepartmentAssignsByManager(dto, ManagerOrEmpId);
                var responnse = new PagingDataModel<CustomerDepartmentAssignViewDTO>(data, dto);
                return GetSuccess(responnse);
            }
            catch (Exception e)
            {
                _logger.LogError("GetCustomerDepartmentAssignsByManager {0} {1}", e.Message, e.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize(Policy = "AdminManagerPolicy")]
        [HttpPost]
        [Route("get-customer-department-assign-by-id/{id}")]
        public async Task<IActionResult> GetCustomerDepartmentAssign([FromRoute] long id)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (IsManager)
                {
                    var hasAccess =
                        await _customerDepartmentAssignBizLogic.ManagerHasAccessToAssignAsync(ManagerOrEmpId, id);
                    if (!hasAccess) return GetForbidden();
                }

                var responnse = await _customerDepartmentAssignBizLogic.GetCustomerDepartmentAssign(id);
                if (responnse == null) return GetNotFound(Constants.GetNotFound);
                return GetSuccess(responnse);
            }
            catch (Exception e)
            {
                _logger.LogError("GetCustomerDepartmentAssign {0} {1}", e.Message, e.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
    }
}