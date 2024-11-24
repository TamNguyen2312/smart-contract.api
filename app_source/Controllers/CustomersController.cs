using App.API.Filter;
using App.BLL.Interfaces;
using App.Entity.DTOs.Customer;
using App.Entity.Entities;
using FS.BaseAPI;
using FS.Commons;
using FS.Commons.Models.DTOs;
using FS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : BaseAPIController
    {
        private readonly ICustomerBizLogic _customerBizLogic;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerBizLogic customerBizLogic, ILogger<CustomersController> logger)
        {
            _customerBizLogic = customerBizLogic;
            _logger = logger;
        }

        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [Route("create-update-customer")]
        public async Task<IActionResult> CreateUpdateCustomer(CustomerRequestDto dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                if (!ModelState.IsValid) return ModelInvalid();
                
                if (!Helpers.IsValidEmail(dto.Email.Trim()))
                {
                    ModelState.AddModelError("Email", Constants.EmailAddressFormatError);
                    return ModelInvalid();
                }

                var existedEmail = await _customerBizLogic.GetCustomerByEmail(dto.Email);
                if (existedEmail != null)
                {
                    return SaveError("Email khách hàng đã tồn tại.");
                }
                var responnse = await _customerBizLogic.CreateUpdateCustomer(dto, UserId);
                if (!responnse.IsSuccess) return SaveError(responnse.Message);
                return SaveSuccess(responnse);
            }
            catch (Exception e)
            {
                _logger.LogError("CreateUpdateCustomer {0} {1}", e.Message, e.StackTrace); 
                return Error(Constants.SomeThingWentWrong);
            }   
        }

        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [Route("get-customers-by-admin")]
        public async Task<IActionResult> GetAllCustomers([FromBody]CustomerGetListDTO dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);
                
                if (!ModelState.IsValid) return ModelInvalid();

                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ.");
                }
                var data = await _customerBizLogic.GetAllCustomers(dto, UserId);
                var response = new PagingDataModel<CustomerViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetAllCustomers {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        
        [FSAuthorize(Policy = "ManagerRolePolicy")]
        [HttpPost]
        [Route("get-customers-by-manager")]
        public async Task<IActionResult> GetCustomersByManager([FromBody]CustomerGetListDTO dto)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);
                
                if (!ModelState.IsValid) return ModelInvalid();

                if (!dto.IsValidOrderDate())
                {
                    ModelState.AddModelError("OrderDate", "OrderDate không hợp lệ.");
                }
                var data = await _customerBizLogic.GetCustomersByManagerAsync(dto, ManagerOrEmpId);
                var response = new PagingDataModel<CustomerViewDTO>(data, dto);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetCustomersByManager {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        

        [FSAuthorize(Policy = "AdminManagerPolicy")]
        [HttpGet]
        [Route("get-customer-by-id/{customerId}")]
        public async Task<IActionResult> GetCustomer([FromRoute]long customerId)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);
                
                if (IsManager)
                {
                    var hasAccess = await _customerBizLogic.ManagerHasAccessToCustomerAsync(ManagerOrEmpId, customerId);
                    if (!hasAccess) return GetForbidden();
                }
                
                var data = await _customerBizLogic.GetCustomer(customerId);
                if (data == null) return GetNotFound(Constants.GetNotFound);
                return GetSuccess(data);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetCustomer {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize(Policy = "ManagerEmployeePolicy")]
        [HttpGet]
        [Route("get-dropdown-customers-by-manager-or-employee")]
        public async Task<IActionResult> GetDropdownCustomersByManagerOrEmployee()
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                var response = new List<CustomerViewDTO>();

                if (IsManager)
                {
                    response = await _customerBizLogic.GetDropdownCustomersByManagerOrEmployee(ManagerOrEmpId, true);
                }
                else
                {
                    response = await _customerBizLogic.GetDropdownCustomersByManagerOrEmployee(ManagerOrEmpId, false);
                }
                
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetDropdownCustomersByManagerOrEmployee {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        [FSAuthorize(Policy = "AdminRolePolicy")]
        [HttpGet]
        [Route("get-dropdown-customers-by-admin/{departmentId}")]
        public async Task<IActionResult> GetDropdownCustomersByAdmin([FromRoute]long departmentId)
        {
            try
            {
                var isInvoked = await IsTokenInvoked();
                if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);

                var response = await _customerBizLogic.GetDropdownCustomersByAdmin(departmentId);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetDropdownCustomersByAdmin {0} {1}", ex.Message, ex.StackTrace);
                return Error(Constants.SomeThingWentWrong);
            }
        }
        
        
        // [Authorize]
        // [HttpPost]
        // [Route("delete-customer/{customerId}")]
        // public async Task<IActionResult> DeleteCustomer([FromRoute]long customerId)
        //
        // {
        //     try
        //     {
        //         var isInvoked = await IsTokenInvoked();
        //         if (isInvoked) return GetUnAuthorized(Constants.GetUnAuthorized);
        //         
        //         var response = await _customerBizLogic.DeleteCustomer(customerId, UserId);
        //         if (!response.IsSuccess) return SaveError(response.Message);
        //         return GetSuccess(response);
        //     }
        //     catch (Exception ex)
        //     {
        //         ConsoleLog.WriteExceptionToConsoleLog(ex);
        //         return Error(Constants.SomeThingWentWrong);
        //     }
        // }
    }
}
